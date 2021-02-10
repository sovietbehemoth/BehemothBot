import requests
import asyncio
import websocket
import _thread
import time
import json

from websocket import create_connection

def CLIENT_GET_URL():
    r = requests.get('https://discord.com/api/v8/gateway')
    resp = json.loads(r.content)
    return resp["url"];

def CLIENT_HELLO(url):
    ws = create_connection(url)
    ws.send('{"v":8, "encoding":"json"}')
    resp = json.loads(ws.recv())
    if resp["op"] == 10: 
        print("Websocket: Hello established")
        print(resp)
    else:
        print("Websocket: Connection failed")

def CLIENT_MESSAGES(url):
    ws = create_connection(url)
    ws.send('{"op":0, }')

def CLIENT_IDENTIFY(url):
    print("Websocket: Identifying client")
    ws = create_connection(url)
    ws.send('{"v":8, "encoding":"json", "op":2, "token":"NzU4MzU3NDYzMTg2OTMxNzg2.X2txbA.6xPFDGzqH9GPSBMmFgxPCqCJIv4", "intents":512"properties"{"$os":"linux", "$browser":"disco", "$device":"disco"}}')
    print(ws.recv())

def CLIENT_HEARTBEAT(url):
    while 1:
     ws = create_connection(url)
     ws.send('{"op": 11, "d":"null"}')
     print(ws.recv())
     time.sleep(41.25)
async def RUN_BOT():
    print(CLIENT_GET_URL())
    CLIENT_HELLO(CLIENT_GET_URL());
    CLIENT_IDENTIFY(CLIENT_GET_URL());
    CLIENT_HEARTBEAT(CLIENT_GET_URL())






asyncio.run(RUN_BOT())
print("d");
if input():
    
    messagescan = input();
    
    auth = {
        'Authorization': 'NzU4MzU3NDYzMTg2OTMxNzg2.X2txbA.6xPFDGzqH9GPSBMmFgxPCqCJIv4'
        }
    payload = {
        'content': messagescan
        }
    r = requests.post('https://discord.com/channels/591116711869022208/768501915104968755', data=payload, headers=auth);
    print(r.status_code);

    
