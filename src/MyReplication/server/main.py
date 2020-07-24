from datetime import date
import decimal
from json import encoder
import sys
import json
import encodings;
import datetime
import requests
from pymysqlreplication import BinLogStreamReader
from pymysqlreplication.constants.FIELD_TYPE import DECIMAL
from pymysqlreplication.row_event import (DeleteRowsEvent, UpdateRowsEvent, WriteRowsEvent)
from datetime import date, datetime
mysql_settings = {'host': '192.168.100.162', 'port': 3306, 'user': 'quantum', 'passwd': 'scmadmin'}

class ComplexEncoder(json.JSONEncoder):
    def default(self, obj):
        if isinstance(obj, datetime):
            return obj.strftime('%Y-%m-%d %H:%M:%S')
        elif isinstance(obj, date):
            return obj.strftime('%Y-%m-%d')
        elif isinstance(obj, decimal.Decimal):
            return str(obj)
        elif isinstance(obj, bytes):
            return obj.decode('utf-8')  
        else:
            return json.JSONEncoder.default(self, obj)

session = requests.session()
session.keep_alive = False 
def readBinLog():
    stream = BinLogStreamReader(
        connection_settings=mysql_settings,
        server_id=3,
        blocking=True,
        only_tables=['order_info','log_info'],
        only_events=[DeleteRowsEvent, WriteRowsEvent, UpdateRowsEvent])

    for binlogevent in stream:
        for row in binlogevent.rows:
            event = { "schema": binlogevent.schema, "table": binlogevent.table, "log_pos": binlogevent.packet.log_pos }
            if isinstance(binlogevent, DeleteRowsEvent):
                event["action"] = "delete"
                event["origin"] = dict(row["values"].items())
                event["current"] = None
                event = dict(event.items())
            elif isinstance(binlogevent, UpdateRowsEvent):
                event["action"] = "update"
                event["origin"] = dict(row["before_values"].items())
                event["current"] = dict(row["after_values"].items())
                event = dict(event.items())
            elif isinstance(binlogevent, WriteRowsEvent):
                event["action"] = "insert"
                event["origin"] = None
                event["current"] = dict(row["values"].items())
                event = dict(event.items())
            sendBinLog(event)
    stream.close()

def sendBinLog(event):
    url = "https://localhost:44348/EventBus/PublishBinLog"
    headers = {
        'Content-Type': "application/json",
    }

    try:
        payload = json.dumps(event,cls=ComplexEncoder)
        print(payload)
        response = session.request("POST", url, data=payload, headers=headers,verify=False)
        print(response.status_code)
        print(response.text)
    except Exception:
        pass

if (__name__ == '__main__'):
    readBinLog();