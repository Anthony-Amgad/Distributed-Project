import random
import socket
import _thread
from pymongo.mongo_client import MongoClient
from pymongo.server_api import ServerApi
import time

names = []
game_started = False

positions = ['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']
state = ["NA","NA","NA","NA"]
rank = [0,0,0,0]
playerSockets = []
gameEnded = False

def db_service():
    global game_started
    db_ip = "ec2-54-162-162-190.compute-1.amazonaws.com"
    port
    # Create a new client and connect to the primary server
    mongoClient = MongoClient(db_ip, 28041)
    db = mongoClient.get_database('racingGameDB')
    records = db.Sessions
    
    lobby_startTime = str(time.time())
    while (game_started):
        records.insert_one({"lobby_start_time": lobby_startTime,
                            "lobby_players":names, 
                            "timestamp": time.time(), 
                            "positions": positions})
        time.sleep(0.01)


def on_new_client(clientsocket,name,num):
    global gameEnded
    global names
    global positions
    global state
    global rank
    global playerSockets
    global s
    connected = True
    state[num] = "waiting"
    while connected :
        try:
            msg = clientsocket.recv(1024).decode('utf-8')
            # print(msg)
            m1, m2 = msg.split('$')
            if m1 == "start":
                seed = random.randint(0,10000)
                for i, p in enumerate(playerSockets):
                    p.send(("start$"+str(seed)).encode('utf-8'))
                    state[i] = "started"
            elif m1 == "pos":
                #print(addr, ' >> ', m2 , ' >> ', len(m2))
                if len(m2) != 0:
                    positions[num] = m2
                msg = "pos$"+str(positions)
                clientsocket.send(msg.encode('utf-8'))
            elif m1 == "chat":
                for p in playerSockets:
                    p.send(("chat$"+name+"$"+m2).encode('utf-8'))
            elif m1 == "finish":
                state[num] = "finished"
                rank[num] = state.count("finished")
                clientsocket.send(("rank$"+str(state.count("finished"))).encode('utf-8'))
                if state.count("started") == 0:
                    gameEnded = True
                    for p in playerSockets:
                        p.send(("end$"+str(rank[0])+str(rank[1])+str(rank[2])+str(rank[3])).encode('utf-8'))
        except Exception as e:
            print(name + " disconnected" + " : " + str(e))
            state[num] = "disconnected"
            if (state.count("disconnected") + state.count("NA")) == 4:
                names = []
                positions = ['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']
                state = ["NA","NA","NA","NA"]
                rank = [0,0,0,0]
                playerSockets = []
                gameEnded = False
                s.send(("end$").encode('utf-8'))
            connected = False
    clientsocket.close()


host = socket.gethostname()
port = 50000


# client = MongoClient(uri, server_api=ServerApi('1'))
# Send a ping to confirm a successful connection
# try:
#     client.admin.command('ping')
#     print("Pinged your deployment. You successfully connected to MongoDB!")
#     collection = client.test.repTest
#     collection.drop()
#     collection.insert_one(dict(name='Foo', age='30'))
#     for x in range(5):
#         try:
#             print('Fetching record: %s' % collection.find_one())
#         except Exception as e:
#             print('Could not connect to primary')
#         time.sleep(3)
# except Exception as e:
#     print(e)


while True:
    s = socket.socket()
    s.connect((host, 50001))
    s.send("server".encode('utf-8'))
    s.recv(1024).decode('utf-8')
    s.send((str(port)+"$wokenup").encode('utf-8'))
    msg = s.recv(1024).decode('utf-8')
    online = False
    running = False
    if msg == "ready":
        online = True
    if online and not running:
        msg = s.recv(1024).decode('utf-8')
        if msg == "running":
            running = True

    UserS = socket.socket()
    UserS.bind((host, port))        # Bind to the port
    UserS.listen(4)               # Now wait for client connection.
    print('Server started!')
    print('Waiting for clients...')
    _thread.start_new_thread(
        db_service)

    while True:
        c, addr = UserS.accept()    # Establish connection with client.
        print('Got connection from', addr)
        name = c.recv(1024).decode('utf-8')
        names.append(name)
        s.send(("name$" + name).encode('utf-8'))
        s.recv(1024).decode('utf-8')
        c.send((str(len(playerSockets)) + "$" + str(names)).encode('utf-8'))
        print(len(playerSockets))
        for p in playerSockets:
            p.send(("join$" + name).encode('utf-8'))
        playerSockets.append(c)
        print(len(playerSockets) - 1)
        _thread.start_new_thread(
            on_new_client, (c, name, len(playerSockets) - 1))

    s.close()
