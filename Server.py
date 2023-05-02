import socket
import _thread

addrs = []
positions = ['(0.0,-4.0,0.0)','(0.0,-6.0,0.0)','(0.0,-8.0,0.0)','(0.0,-10.0,0.0)']
count = 0

def on_new_client(clientsocket,addr,num):
    connected = True
    while connected :
        try:
            msg = clientsocket.recv(1024).decode('utf-8')
            #do some checks and if msg == someWeirdSignal: break:
            print(addr, ' >> ', msg)
            positions[num] = msg
            msg = str(positions)
            #Maybe some code to compute the last digit of PI, play game or anything else can go here and when you are done.
            clientsocket.send(msg.encode('utf-8'))
        except:
            connected = False
    clientsocket.close()


s = socket.socket()
host = socket.gethostname()
port=50000


print('Server started!')
print('Waiting for clients...')

s.bind((host, port))        # Bind to the port
s.listen(4)                 # Now wait for client connection.                            

while True:
    c, addr = s.accept()    # Establish connection with client.
    print('Got connection from', addr)
    c.send(str(count).encode('utf-8'))
    addrs.append(addr)
    _thread.start_new_thread(on_new_client,(c,addr,count))
    count+=1
s.close() 