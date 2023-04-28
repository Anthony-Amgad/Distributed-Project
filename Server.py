import socket
import _thread

addrs = []
positions = ['x','x','x','x']
count = 0

def on_new_client(clientsocket,addr,num):
    while True:
        msg = clientsocket.recv(1024).decode('utf-8')
        #do some checks and if msg == someWeirdSignal: break:
        print(addr, ' >> ', msg)
        positions[num] = msg
        for i in range(0,4):
            if (i != num) and (positions[i] != 'x'):
                msg = str(positions[i])
                #Maybe some code to compute the last digit of PI, play game or anything else can go here and when you are done.
                clientsocket.send(msg.encode('utf-8'))
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
    addrs.append(addr)
    _thread.start_new_thread(on_new_client,(c,addr,count))
    count+=1
    #Note it's (addr,) not (addr) because second parameter is a tuple
    #Edit: (c,addr)
    #that's how you pass arguments to functions when creating new threads using thread module.
s.close() 