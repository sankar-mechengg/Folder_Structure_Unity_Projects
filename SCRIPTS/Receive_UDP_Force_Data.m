% Define the port number to match the sender's port
port = 4400;

% Create a UDP object to receive data on the specified port
udpObj = udpport("LocalPort", port);

% Display that we're waiting for data
disp(['Waiting for data on UDP port ', num2str(port), '...']);

% Continuously read data from the UDP port
while true
    % Check for the availability of the data
    dataAvailable = udpObj.NumBytesAvailable;
    
    if dataAvailable > 0
        % Read the data
        data = read(udpObj, dataAvailable, "string");
        
        % Split the received data into X, Y, Z components
        forceValue = str2double(data);
              
        % Display the received angles
        disp(['Received force: F=', num2str(forceValue)]);
    end
    
    % Small pause to not overload the loop
    pause(0.1);
end
