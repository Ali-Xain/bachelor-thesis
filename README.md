# Vision Based Cusrsor Control Using Hand Getures

Vision Based Virtual Mouse is a Desktop Application that shall enable the users to interact more naturally with their computers through hand gestures without using any additional harware or glove at all. It allow all users to perform mouse functions using hand gestures by utilizing the best out of webcam. 

# Demo

Check the demo available at https://tinyurl.com/uleo5db

# Functionality
1) Control the movement of mouse

2) Perform the mouse functions using hand gestures such as right click, left click from 13-15 inches distance 

3) No more delay then 0.1 sec. Concurrent execution 

4) Do not crash. System is robust enough to keep working in case of unknown gesture 

5) Multiple windows management 

6) User can interact more naturally 

7) Switch the control back to mouse anytime if needed 



# Set Up Enviornment
Well light room 

Test Webcam 

Install Emgucv 

Install Visual Studio 
Download dll files from https://drive.google.com/drive/folders/1xdh7elB03-BHvPUdvlDd-fJv2No1-IWc and placed under debug folder and inside HandGestureRecognition directory.

Add dll and other libraries in Home_path

Open the project and execute it or run just the exe file 


# Assumptions
It is assumed that the user has a computer with a working webcam. The program will not work without a working webcam. The application will automatically attempt to turn on the webcam and if this fails the user will be prompted to manually turn on the computers webcam. It is also assumed that the resolution of the webcam is at least 480 pixels. 

Software must be used in a well lit room. If the lighting condition is too poor, then the accuracy of the program sharply drops to a point where it is unusable. The application uses shape analysis to recognize the hand gesture, thus the users hand must be within 13-15 inhces distance from the webcam. The proposed system can only work with windows operating system. 

The proposed system uses a single webcam placed in front of the user so if any object in front of the hand or if there is any shadows forecasted on the hand then the system will not work. 
User hand shall not be rotated or tilted while performing hand gestures otherwise it will effect accuracy 


# Gesture-Operation

After clicking on Cursor control button you 'll be able to perform following operations. 


Gesture 1:       Control the Movement of Mouse 

Gesture 2:       Left Mouse Click 

Gesture 3:       Right Mouse click 

Gesture 4:       Open Task manager 

Gesture 5:       Exit Application 

Gesture 6:       Scroll Down 

Gesture 7:       Scroll up 
Gesture 8:       Minimize current window 

Gesture 9:       Maximize the last minimized window 

Gesture 10:      Exit Application 

