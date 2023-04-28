import socket
s = socket.socket()
host = socket.gethostname()
port=50000

print('Server started!')
print('Waiting for clients...')

s.bind((host, port))        # Bind to the port
s.listen()                 # Now wait for client connection.
c, addr = s.accept()     # Establish connection with client.
print('Got connection from', addr)
while True:
    try:
        msg = c.recv(1024).decode('utf-8')
        print(addr, ' >> ', msg)
        c.send(msg.encode('utf-8'))
    except:
        c.close()  