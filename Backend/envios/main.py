from fastapi import FastAPI
from pydantic import BaseModel
from typing import List
from kafka import KafkaProducer
import json
import requests

api = FastAPI()

# ===============================
#  CONFIGURACIÓN ENVIOCLICK
# ===============================
ENVIOCLICK_URL = "https://api.envioclickpro.com.co/api/v2/quotation"
API_KEY = "1beae26b-10e4-4cad-a916-7f5f4a54843e"  # <-- reemplaza con tu API key


# ===============================
#  MODELOS Pydantic
# ===============================

producer = KafkaProducer(
    bootstrap_servers=['kafka:9092'],  # Ajusta según tu docker-compose
    value_serializer=lambda v: json.dumps(v).encode('utf-8')
)

KAFKA_TOPIC = "notification-envios"

class Package(BaseModel):
    weight: float
    height: int
    width: int
    length: int


class Location(BaseModel):
    daneCode: str
    address: str


class QuotationRequest(BaseModel):
    email: str
    packages: List[Package]
    description: str
    contentValue: float
    codValue: float
    includeGuideCost: bool
    codPaymentMethod: str
    origin: Location
    destination: Location

def elegir_rate_mas_barato(rates):
    return min(rates, key=lambda r: r.get("flete", 999999))

def publicar_en_kafka(email, rate):
    mensaje = {
        "email": email,
        "selectedRate": {
            "idRate": rate["idRate"],
            "product": rate["product"],
            "carrier": rate["carrier"],
            "flete": rate["flete"],
            "deliveryDays": rate["deliveryDays"]
        }
    }

    producer.send(KAFKA_TOPIC, mensaje)
    producer.flush()

    print("📤 Kafka message enviado:", mensaje)


# ===========================================
#  JSON QUEMADO (por si deseas probar directo)
# ===========================================
JSON_QUEMADO = {
    "packages": [
        {
            "weight": 1,
            "height": 10,
            "width": 5,
            "length": 15
        }
    ],
    "description": "descripción",
    "contentValue": 12000,
    "codValue": 12000,
    "includeGuideCost": False,
    "codPaymentMethod": "cash",
    "origin": {
        "daneCode": "11001000",
        "address": "Calle 98 62-37"
    },
    "destination": {
        "daneCode": "11001000",
        "address": "Carrera 46 # 93 - 45"
    }
}


# ===============================
#  ENDPOINT PRINCIPAL
# ===============================
@api.post("/api/envios/cotizar")
def cotizar_envio(request: QuotationRequest):

    payload = request.dict()
    email_cliente = payload.pop("email")

    headers = {
        "Content-Type": "application/json",
        "Authorization": API_KEY
    }

    response = requests.post(
        ENVIOCLICK_URL,
        json=payload,
        headers=headers,
        timeout=10
    )

    data = response.json()

    rates = data.get("data", {}).get("rates", [])

    if not rates:
        return {
            "status": "ERROR",
            "message": "No se recibieron tarifas desde EnvioClick"
        }
    
    rate_seleccionado = elegir_rate_mas_barato(rates)

    publicar_en_kafka(email_cliente, rate_seleccionado)

    return {
        "status": "OK",
        "message": "Cotización procesada y mensaje enviado a Kafka",
        "rate_usado": {
            "carrier": rate_seleccionado["carrier"],
            "product": rate_seleccionado["product"],
            "flete": rate_seleccionado["flete"]
        }
    }


# =======================================
#  ENDPOINT PARA PROBAR JSON QUEMADO
# =======================================
@api.get("/api/envios/cotizar/test")
def cotizar_test():

    headers = {
        "Content-Type": "application/json",
        "Authorization": API_KEY
    }

    response = requests.post(
        ENVIOCLICK_URL,
        json=JSON_QUEMADO,
        headers=headers,
        timeout=10
    )

    return {
        "status_code": response.status_code,
        "json_enviado": JSON_QUEMADO,
        "respuesta_envio_click": response.json()
    }
