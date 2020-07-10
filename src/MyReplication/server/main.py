from datetime import date
import decimal
import sys
import json
import datetime
from pymysqlreplication import BinLogStreamReader
from pymysqlreplication.constants.FIELD_TYPE import DECIMAL
from pymysqlreplication.row_event import (DeleteRowsEvent, UpdateRowsEvent, WriteRowsEvent)
mysql_settings = {'host': '192.168.100.212', 'port': 3306, 'user': 'quantum', 'passwd': 'CS212SJK'}


stream = BinLogStreamReader(
    connection_settings=mysql_settings,
    server_id=3,
    blocking=True,
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
stream.close()
