import socket
import _thread
from pymongo.mongo_client import MongoClient
from pymongo.server_api import ServerApi



NamesConnected = {"admin":"admin"}
ReadyGameServers = {"0":['0','0','0','0']}
InGameServers = {"0":['0','0','0','0']}
PlayingGameServers = {"0":['0','0','0','0']}
s = socket.socket()
host = socket.gethostname()
port=50001
OfflineGameServers = {"0", '192.168.56.1#50002', '192.168.56.1#50003', '192.168.56.1#50004', '192.168.56.1#50005', '192.168.56.1#50006', '192.168.56.1#50007'}
GameServersLobbyIDS = {"0":"0", '192.168.56.1#50002':"0", '192.168.56.1#50003':"0", '192.168.56.1#50004':"0", '192.168.56.1#50005':"0", '192.168.56.1#50006':"0", '192.168.56.1#50007':"0"}
#ygf ip 192.168.1.32
#antoon ip 192.168.56.1
#kero ip 192.168.126.1

def on_new_server(serversocket, Sname):
    connected = True
    serversocket.send("running".encode('utf-8'))
    while connected :
        try:
            msg = serversocket.recv(1024).decode('utf-8')
            msgs = msg.split('$')
            if msgs[0] == "name":
                if Sname in PlayingGameServers:
                    PlayingGameServers[Sname].append(msgs[1])
                    NamesConnected[msgs[1]] = 'inlobby'
                serversocket.send("ok".encode('utf-8'))
                print(msgs[1])
            elif msgs[0] == "end": 
                if Sname in PlayingGameServers:
                    for n in PlayingGameServers[Sname]:
                        NamesConnected.pop(n)
                    PlayingGameServers.pop(Sname)
                if Sname in InGameServers:
                    for n in InGameServers[Sname]:
                        NamesConnected.pop(n)
                    InGameServers.pop(Sname)
                ReadyGameServers.update({Sname : []})
                print(PlayingGameServers)
                print(ReadyGameServers)
            elif msgs[0] == 'startgame':
                for client in PlayingGameServers[Sname]:
                    NamesConnected[client] = 'ingame'
                InGameServers.update({Sname: PlayingGameServers[Sname]})
                PlayingGameServers.pop(Sname)
            elif msgs[0] == 'dc':
                if Sname in InGameServers and msgs[1] in InGameServers[Sname]:
                    NamesConnected[msgs[1]] = 'disconnected'
                elif Sname in PlayingGameServers and msgs[1] in PlayingGameServers[Sname]:
                    PlayingGameServers[Sname].remove(msgs[1]) 
            elif msgs[0] == "dblobbyid":
                GameServersLobbyIDS[Sname] = msgs[1]
                print(msgs[1])
        except Exception as e:
            print(e)
            if Sname in PlayingGameServers:
                PlayingGameServers.pop(Sname)
            if Sname in InGameServers:
                InGameServers.pop(Sname)
            if Sname in ReadyGameServers:
                ReadyGameServers.pop(Sname)
            OfflineGameServers.add(Sname)
            connected = False
    print("connectionLost")
    serversocket.close()

def on_new_client(clientsocket, name):
    connected = True
    while connected :
        try:
            msg = clientsocket.recv(1024).decode('utf-8')
            msgs = msg.split('$')
            if msgs[0] == "new":
                if len(ReadyGameServers) > 1:
                    server = list(ReadyGameServers.keys())[1]
                    ReadyGameServers.pop(server)
                    PlayingGameServers.update({server : []})
                    clientsocket.send(server.encode('utf-8'))
                else:
                    clientsocket.send("sc".encode('utf-8'))
            elif msgs[0] == "join":
                if len(PlayingGameServers[msgs[1]]) < 4:
                    clientsocket.send("ok".encode('utf-8'))
        except:
            if NamesConnected[name] == 'ingame':
                NamesConnected[name] = 'disconnected'
            else:
                NamesConnected.pop(name)
            connected = False
    clientsocket.close()

db_ip = "ec2-54-162-162-190.compute-1.amazonaws.com"
db_port = 28041
# Create a new client and connect to the primary server
mongoClient = MongoClient(db_ip, db_port)
db = mongoClient.get_database('racingGameDB')
posRecords = db.Positions

print('Server started!')
print('Waiting for clients...')

s.bind((host, port))        # Bind to the port
s.listen(5)                 # Now wait for client connection.                            

while True:
    c, addr = s.accept()    # Establish connection with client.
    print('Got connection from', addr)
    msg = c.recv(1024).decode('utf-8')
    if msg == "server":
        c.send("ok".encode('utf-8'))
        msg = c.recv(1024).decode('utf-8')
        msg = msg.split('$')
        if msg[1] == "wokenup":
            OfflineGameServers.remove(addr[0]+'#'+msg[0])
        ReadyGameServers.update({(addr[0]+'#'+msg[0]) : []})
        c.send("ready".encode('utf-8'))
        _thread.start_new_thread(on_new_server,(c,(addr[0]+'#'+msg[0])))
    elif msg == "user":
        c.send("ok".encode('utf-8'))
        msg = c.recv(1024).decode('utf-8').lower()
        if msg not in NamesConnected:
            NamesConnected.update({msg : "searching"})
            c.send(("ready$" + str(PlayingGameServers)).encode('utf-8'))
            _thread.start_new_thread(on_new_client,(c,msg))
        elif NamesConnected[msg] == 'disconnected':
            found = 0
            for key in InGameServers.keys():
                if msg in InGameServers[key]:
                    positions = posRecords.find({'lobby_id': GameServersLobbyIDS[key]})
                    positions = positions.sort('timestamp', -1)
                    print(positions[0]['positions'])
                    c.send(('ingame$' + key + '$' + str(positions[0]['positions'])).encode('utf-8'))
                    found = 1
                    break
            if found == 1:
                NamesConnected[msg] = 'ingame'
        else:
            c.send("AU".encode('utf-8'))
            c.close
    
s.close() 