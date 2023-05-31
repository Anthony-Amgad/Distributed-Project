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
    finished = 0  

    def __init__(self):
        self.names = []
        self.game_started = False
        self.seed = 0
        self.positions = ['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']
        self.state = ["NA","NA","NA","NA"]
        self.rank = [0,0,0,0]
        self.playerSockets = []
        self.gameEnded = False
        self.serverSocket = None
        self.finished = 0   
    
    def startGameServer(self,port): 
        self.names = []
        self.game_started = False
        self.seed = 0
        self.positions = ['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']
        self.state = ["NA","NA","NA","NA"]
        self.rank = [0,0,0,0]
        self.playerSockets = []
        self.gameEnded = False
        self.serverSocket = None
        self.finished = 0   
        while True:
            host = socket.gethostname()
            #matchS = socket.gethostbyname("ec2-54-196-191-35.compute-1.amazonaws.com") #fordeployment
            matchS = "192.168.56.1"
            self.serverSocket = socket.socket()
            self.serverSocket.connect((matchS, 50001))
            self.serverSocket.send("server".encode('utf-8'))
            self.serverSocket.recv(1024).decode('utf-8')
            self.serverSocket.send((str(port)+"$"+matchS).encode('utf-8'))
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

            _thread.start_new_thread(self.db_service,()) #DATABASE START

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
                    self.playerSockets[names_cnt] = c
                    alrd = True
                else:
                    self.names.append(name)
                    names_cnt = len(self.names) - 1
                    c.send((str(names_cnt) + "$" + str(self.names)).encode('utf-8'))
                    for p in self.playerSockets:
                        try:
                            p.send(("join$" + name+"~").encode('utf-8'))
                        except:
                            pass
                    self.state[names_cnt] = "waiting"
                    self.playerSockets.append(c)
                _thread.start_new_thread(
                    self.on_new_client, (c, name, names_cnt, alrd))

            self.serverSocket.close()

    def on_new_client(self,clientsocket,name,num,alr):
        connected = True
        if alr:
            clientsocket.send(("start$"+str(self.seed)+"~").encode('utf-8'))
        while connected :
            try:
                msg = clientsocket.recv(1024).decode('utf-8')
                if not msg: raise
                for token in msg.split('~'):
                    if len(token) > 1:
                        m = token.split('$')
                        if m[0] == "start":
                            self.seed = random.randint(0,10000)
                            for i, p in enumerate(self.playerSockets):
                                try:
                                    p.send(("start$"+str(self.seed)+"~").encode('utf-8'))
                                    self.state[i] = "started"
                                except:
                                    pass
                            self.serverSocket.send(("startgame$").encode('utf-8'))
                            self.game_started = True
                        elif m[0] == "pos":
                            #print(msg)
                            #print(addr, ' >> ', m2 , ' >> ', len(m2))
                            if len(m[1]) != 0:
                                self.positions[num] = m[1]
                            nmsg = "pos$"+str(self.positions)
                            clientsocket.send((nmsg+"~").encode('utf-8'))
                        elif m[0] == "chat":
                            print(m)
                            for p in self.playerSockets:
                                try:
                                    p.send(("chat$"+name+"$"+m[1]+"~").encode('utf-8'))
                                except:
                                    pass
                        elif m[0] == "finish":
                            self.state[num] = "finished"
                            self.finished += 1
                            self.rank[num] = int(self.finished)
                            clientsocket.send(("rank$"+str(self.finished)+"~").encode('utf-8'))
                            if self.state.count("started") == 0:
                                #print(self.state)
                                self.gameEnded = True
                                for p in self.playerSockets:
                                    try:
                                        p.send(("end$"+str(self.rank[0])+str(self.rank[1])+str(self.rank[2])+str(self.rank[3])+"~").encode('utf-8'))
                                    except:
                                        pass
            except Exception as e:
                print(name + " disconnected" + " : " + str(e))
                self.state[num] = "disconnected"
                if not self.game_started:
                    self.playerSockets.remove(clientsocket)
                    self.names.remove(name)
                    self.state.pop(num)
                    self.state.append("NA")
                for i, p in enumerate(self.playerSockets):
                    if p != clientsocket:
                        try:
                            p.send(("dc$"+name+"$"+str(i)).encode('utf-8'))
                        except:
                            print("pass")
                self.serverSocket.send(('dc$' + name).encode('utf-8'))
                if (self.state.count("disconnected") + self.state.count("NA")) == 4:
                    self.game_started = False
                    self.names = []
                    print(self.names)
                    self.positions = ['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']
                    self.state = ["NA","NA","NA","NA"]
                    self.rank = [0,0,0,0]
                    self.playerSockets = []
                    self.gameEnded = False
                    print("hehehe")
                    self.serverSocket.send(("end$").encode('utf-8'))
                connected = False
        clientsocket.close()

    def db_service(self):
        while True: 
            db_ip = "ec2-54-162-162-190.compute-1.amazonaws.com"
            db_port = 28041
            # Create a new client and connect to the primary server
            mongoClient = MongoClient(db_ip, db_port)
            db = mongoClient.get_database('racingGameDB')
            posRecords = db.Positions
            lobbyRecords = db.Sessions
            while(not self.game_started):
                pass
            lobby_id = str(uuid.uuid4())
            lobbyRecords.insert_one({"lobby_id": lobby_id,
                                    "lobby_players":self.names})
            self.serverSocket.send(("dblobbyid$"+lobby_id).encode('utf-8'))
            #currentTime = time.time()       
            while(self.game_started):
                posRecords.insert_one({"lobby_id": lobby_id, 
                                    "timestamp": time.time(), 
                                    "positions": self.positions})
                #print(str(time.time()-currentTime))
                #currentTime = time.time()
                    #time.sleep(0.01)



#MAIN
if __name__ == "__main__":
    gS = []
    for i, j in enumerate(range(50002, 50003)):
        gS.append(GameServer())
        _thread.start_new_thread(gS[i].startGameServer, tuple([j]))
    

    while True:
        pass
    