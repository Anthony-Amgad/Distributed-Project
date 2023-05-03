import socket
import _thread

NamesConnected = {"admin":"admin"}
ReadyGameServers = {"0":['0','0','0','0']}
PlayingGameServers = {"0":['0','0','0','0']}
s = socket.socket()
host = socket.gethostname()
port=50001
OfflineGameServers = {"0", '192.168.56.1#50000'}


def on_new_server(serversocket, Sname):
    connected = True
    serversocket.send("running".encode('utf-8'))
    while connected :
        try:
            msg = serversocket.recv(1024).decode('utf-8')
            msgs = msg.split('$')
            if msgs[0] == "name":
                PlayingGameServers[Sname].append(msgs[1])
                serversocket.send("ok".encode('utf-8'))
        except:
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
            connected = False
    clientsocket.close()



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
        elif msg[1] == "ended":
            PlayingGameServers.pop(addr[0]+'#'+msg[0])
        ReadyGameServers.update({(addr[0]+'#'+msg[0]) : []})
        c.send("ready".encode('utf-8'))
        _thread.start_new_thread(on_new_server,(c,(addr[0]+'#'+msg[0])))
    elif msg == "user":
        c.send("ok".encode('utf-8'))
        msg = c.recv(1024).decode('utf-8').lower()
        Names = set(NamesConnected.keys())
        if len(Names.intersection({msg})) == 0:
            NamesConnected.update({msg : "searching"})
            c.send(("ready$" + str(PlayingGameServers)).encode('utf-8'))
            _thread.start_new_thread(on_new_client,(c,msg))
        else:
            c.send("AU".encode('utf-8'))
            c.close
    
s.close() 