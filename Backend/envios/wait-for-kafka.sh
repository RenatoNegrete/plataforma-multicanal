#!/bin/sh

echo "⏳ Esperando 10 segundos antes de iniciar el servicio..."
sleep 20

echo "✅ Tiempo de espera completado. Iniciando servicio..."
exec "$@"
