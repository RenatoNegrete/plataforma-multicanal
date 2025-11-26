import json
import asyncio
from fastapi import FastAPI
from aiokafka import AIOKafkaConsumer
from prometheus_fastapi_instrumentator import Instrumentator
import sib_api_v3_sdk
from sib_api_v3_sdk.rest import ApiException

# =======================
# BREVO CONFIG
# =======================

configuration = sib_api_v3_sdk.Configuration()
configuration.api_key['api-key'] = "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"

brevo_client = sib_api_v3_sdk.TransactionalEmailsApi(
    sib_api_v3_sdk.ApiClient(configuration)
)

# =======================
# FASTAPI APP
# =======================

app = FastAPI()

@app.get("/api/notificaciones/status")
def root():
    return {"status": "Kafka Email Service Running"}

Instrumentator().instrument(app).expose(app)

# =======================
# EMAIL SENDER FUNCTION
# =======================

def send_email(to_email: str, subject: str, html: str):

    email = sib_api_v3_sdk.SendSmtpEmail(
        to=[{"email": to_email}],
        sender={"email": "eduardodebrigard1@gmail.com", "name": "Plataforma Multicanal"},
        subject=subject,
        html_content=html
    )

    try:
        brevo_client.send_transac_email(email)
        print(f"📨 Email enviado a {to_email}")
    except ApiException as e:
        print(f"❌ Error enviando correo a {to_email}: {e}")


# =======================
# KAFKA CONSUMER TASK
# =======================

async def manejar_notificacion_pago(data):
    mail = data.get("mail")
    transaction_id = data.get("transactionId")
    message = data.get("message")

    html = f"""
    <html>
        <body>
            <h2>Estado de tu Transacción</h2>
            <p><b>ID:</b> {transaction_id}</p>
            <p>{message}</p>
        </body>
    </html>
    """

    send_email(mail, "Notificación de Pago", html)

async def manejar_cotizacion_envio(data):
    email = data.get("email")
    rate = data.get("selectedRate", {})

    carrier = rate.get("carrier", "N/A")
    product = rate.get("product", "N/A")
    flete = rate.get("flete", "N/A")
    days = rate.get("deliveryDays", "N/A")

    html = f"""
    <html>
        <body>
            <h2>Envío de tu orden</h2>
            <p>Información de tu envío:</p>

            <ul>
                <li><b>Transportadora:</b> {carrier}</li>
                <li><b>Servicio:</b> {product}</li>
                <li><b>Costo:</b> {flete} COP</li>
                <li><b>Días estimados:</b> {days}</li>
            </ul>
        </body>
    </html>
    """

    send_email(email, "Resultado de tu Cotización", html)

async def consume_kafka_messages():

    consumer = AIOKafkaConsumer(
        "notifications-payment",
        "notification-envios",
        bootstrap_servers="kafka:9092",
        group_id="email-service",
        value_deserializer=lambda m: json.loads(m.decode("utf-8"))
    )

    await consumer.start()
    print("📡 Kafka Consumer iniciado, escuchando mensajes...")

    try:
        async for msg in consumer:
            topic = msg.topic
            data = msg.value
            print(f"📥 Mensaje recibido en {topic}: {data}")

            if topic == "notifications-payment":
                await manejar_notificacion_pago(data)

            elif topic == "notification-envios":
                await manejar_cotizacion_envio(data)

    finally:
        await consumer.stop()


# =======================
# STARTUP EVENT
# =======================

@app.on_event("startup")
async def startup_event():
    # Run Kafka consumer in background
    asyncio.create_task(consume_kafka_messages())
