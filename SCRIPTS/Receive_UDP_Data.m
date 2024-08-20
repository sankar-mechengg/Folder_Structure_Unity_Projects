% Define the port number to match the sender's port
port = 7400;

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
        angleValues = str2double(split(data, '&'));
        
        % Extract individual angles
        angleX = angleValues(1);
        angleY = angleValues(2);
        angleZ = angleValues(3);
        
        % Display the received angles
        disp(['Received angles: X=', num2str(angleX), ', Y=', num2str(angleY), ', Z=', num2str(angleZ)]);
    end
    
    % Small pause to not overload the loop
    pause(0.1);
end

% Note: To stop this script, you will need to press Ctrl+C in the Command Window.
