import random
import socket
import _thread

names = []
positions = ['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']
count = 0
state = []
playerSockets = []


def on_new_client(clientsocket,name,num):
    connected = True
    state.append("waiting")
    while connected :
        try:
            msg = clientsocket.recv(1024).decode('utf-8')
            # print(msg)
            m1,m2 = msg.split('$')
            if m1 == "start":
                seed = random.randint(0,10000)
                for p in playerSockets:
                    p.send(("start$"+str(seed)).encode('utf-8'))
            elif m1 == "pos":
                #print(addr, ' >> ', m2 , ' >> ', len(m2))
                if len(m2) != 0:
                    positions[num] = m2
                msg = "pos$"+str(positions)
                clientsocket.send(msg.encode('utf-8'))
            elif m1 == "chat":
                for p in playerSockets:
                    p.send(("chat$"+name+"$"+m2).encode('utf-8'))
        except Exception as e:
            print (e)
            connected = False
            print("dis"+str(num))
    clientsocket.close()



host = socket.gethostname()
port=50000
while True:
    s = socket.socket()
    s.connect((host,50001))
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
        _thread.start_new_thread(on_new_client,(c,name,len(playerSockets) - 1))
    s.close() 