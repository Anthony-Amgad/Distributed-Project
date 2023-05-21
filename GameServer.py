import random
import socket
import _thread
from pymongo.mongo_client import MongoClient
from pymongo.server_api import ServerApi
import time
import uuid





class GameServer:
    names = []
    game_started = False
    seed = 0
    positions = ['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']
    state = ["NA","NA","NA","NA"]
    rank = [0,0,0,0]
    playerSockets = []
    gameEnded = False
    serverSocket = None

    def newGameServer(self,port):    
        while True:
            host = socket.gethostname()
            self.serverSocket = socket.socket()
            self.serverSocket.connect((host, 50001))
            self.serverSocket.send("server".encode('utf-8'))
            self.serverSocket.recv(1024).decode('utf-8')
            self.serverSocket.send((str(port)+"$wokenup").encode('utf-8'))
            msg = self.serverSocket.recv(1024).decode('utf-8')
            online = False
            running = False
            if msg == "ready":
                online = True
            if online and not running:
                msg = self.serverSocket.recv(1024).decode('utf-8')
                if msg == "running":
                    running = True

            UserS = socket.socket()
            UserS.bind((host, port))        # Bind to the port
            UserS.listen(4)               # Now wait for client connection.
            print('Server started!')
            print('Waiting for clients...')

            #_thread.start_new_thread(self.db_service,()) #DATABASE START

            while True:
                c, addr = UserS.accept()    # Establish connection with client.
                print('Got connection from', addr)
                name = c.recv(1024).decode('utf-8')
                self.serverSocket.send(("name$" + str(name)).encode('utf-8'))
                self.serverSocket.recv(1024).decode('utf-8')
                names_cnt = 0
                alrd = False
                if name in self.names:
                    names_cnt = self.names.index(name)
                    self.state[names_cnt] = 'started'
                    c.send((str(names_cnt) + "$" + str(self.names)).encode('utf-8'))
                    alrd = True
                else:
                    self.names.append(name)
                    names_cnt = len(self.names) - 1
                    c.send((str(names_cnt) + "$" + str(self.names)).encode('utf-8'))
                    for p in self.playerSockets:
                        p.send(("join$" + name).encode('utf-8'))
                    self.state[names_cnt] = "waiting"
                self.playerSockets.append(c)
                _thread.start_new_thread(
                    self.on_new_client, (c, name, names_cnt, alrd))

            self.serverSocket.close()

    def on_new_client(self,clientsocket,name,num,alr):
        connected = True
        if alr:
            clientsocket.send(("start$"+str(self.seed)).encode('utf-8'))
        while connected :
            try:
                msg = clientsocket.recv(1024).decode('utf-8')
                # print(msg)
                m1, m2 = msg.split('$')
                if m1 == "start":
                    self.seed = random.randint(0,10000)
                    for i, p in enumerate(self.playerSockets):
                        p.send(("start$"+str(self.seed)).encode('utf-8'))
                        self.state[i] = "started"
                    self.serverSocket.send(("startgame$").encode('utf-8'))
                    self.game_started = True
                elif m1 == "pos":
                    #print(addr, ' >> ', m2 , ' >> ', len(m2))
                    if len(m2) != 0:
                        self.positions[num] = m2
                    msg = "pos$"+str(self.positions)
                    clientsocket.send(msg.encode('utf-8'))
                elif m1 == "chat":
                    for p in self.playerSockets:
                        p.send(("chat$"+name+"$"+m2).encode('utf-8'))
                elif m1 == "finish":
                    self.state[num] = "finished"
                    self.rank[num] = self.state.count("finished")
                    clientsocket.send(("rank$"+str(self.state.count("finished"))).encode('utf-8'))
                    if self.state.count("started") == 0:
                        #print(self.state)
                        self.gameEnded = True
                        for p in self.playerSockets:
                            p.send(("end$"+str(self.rank[0])+str(self.rank[1])+str(self.rank[2])+str(self.rank[3])).encode('utf-8'))
            except Exception as e:
                print(name + " disconnected" + " : " + str(e))
                self.state[num] = "disconnected"
                self.serverSocket.send(('dc$' + name).encode('utf-8'))
                if (self.state.count("disconnected") + self.state.count("NA")) == 4:
                    print("hehehe")
                    self.game_started = False
                    self.names = []
                    self.positions = ['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']
                    self.state = ["NA","NA","NA","NA"]
                    self.rank = [0,0,0,0]
                    self.playerSockets = []
                    self.gameEnded = False
                    self.serverSocket.send(("end$").encode('utf-8'))
                connected = False
        clientsocket.close()

    def db_service(self):
        while True: 
            db_ip = "ec2-54-162-162-190.compute-1.amazonaws.com"
            port = 28041
            # Create a new client and connect to the primary server
            mongoClient = MongoClient(db_ip, port)
            db = mongoClient.get_database('racingGameDB')
            posRecords = db.Positions
            lobbyRecords = db.Sessions
            while(not self.game_started):
                pass
            lobby_id = str(uuid.uuid4())
            lobbyRecords.insert_one({"lobby_id": lobby_id})
            self.serverSocket.send(("dblobbyid$"+lobby_id).encode('utf-8'))
            #currentTime = time.time()       
            while(self.game_started):
                posRecords.insert_one({"lobby_start_time": lobby_id,
                                    "lobby_players":self.names, 
                                    "timestamp": time.time(), 
                                    "positions": self.positions})
                #print(str(time.time()-currentTime))
                #currentTime = time.time()
                    #time.sleep(0.01)



#MAIN

for i in range(50002, 50008):
    gameS = GameServer()
    _thread.start_new_thread(
                    gameS.newGameServer, tuple([i]))
    #time.sleep(2)
    
while True:
    pass
    