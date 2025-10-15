import requests
import json
import os
from typing import Optional, Dict, Any

from fastapi import FastAPI, Body
from fastapi.responses import HTMLResponse
from fastapi.responses import JSONResponse
from fastapi.encoders import jsonable_encoder

from kafka import KafkaConsumer
from kafka import KafkaProducer

kafkaUrl = os.getenv("KAFKA_URL")
consumer = KafkaConsumer(
    "user-events",
    bootstrap_servers=kafkaUrl,
    group_id='user-events-group',
    auto_offset_reset='earliest'
)
#for msg in consumer:
#    print(f"[{msg.topic}] partition={msg.partition}, "
#          f"offset={msg.offset}, value={msg.value.decode()}",
#          flush=True)
    
app = FastAPI()
print(f'kafkaUrl: {kafkaUrl}')

def delivery_report(err, msg):
    if err is not None:
        print(f'Ошибка доставки сообщения: {err}')
    else:
        print(f'Сообщение доставлено в {msg.topic()} [{msg.partition()}]')
    
@app.get("/api/events/health")
def get_health():
    json_data = jsonable_encoder({"status":True})
    return JSONResponse(content=json_data)

@app.post("/api/events/user")
def post_events_user(data  = Body()):
    print(data)
    topic = "user-events"
    producer = KafkaProducer(bootstrap_servers=kafkaUrl)
    message = f'Message {data["user_id"]}'.encode()
    producer.send(topic, message)
    producer.flush()
    print(f"Sent: {message.decode()}", flush=True)
    print(f'Message {data["user_id"]}')
    json_data = jsonable_encoder(data)
    return JSONResponse(content=json_data, status_code=201)

@app.post("/api/events/payment")
def post_events_payment(data  = Body()):
    print(data)
    topic = "payment-events"
    producer = KafkaProducer(bootstrap_servers=kafkaUrl)
    message = f'Message {data["payment_id"]}'.encode()
    future = producer.send(topic, message)
    result = future.get(timeout=60)
    print(result)
    producer.flush()
    print(f"Sent: {message.decode()}", flush=True)
    print(f'Message {data["payment_id"]}')
    json_data = jsonable_encoder(data)
    return JSONResponse(content=json_data, status_code=201)

@app.post("/api/events/movie")
def post_events_movie(data  = Body()):
    print(data)
    topic = "movie-events"
    producer = KafkaProducer(bootstrap_servers=kafkaUrl)
    message = f'Message {data["movie_id"]}'.encode()
    producer.send(topic, message)
    producer.flush()
    print(f"Sent: {message.decode()}", flush=True)
    print(f'Message {data["movie_id"]}')
    json_data = jsonable_encoder(data)
    return JSONResponse(content=json_data, status_code=201)
