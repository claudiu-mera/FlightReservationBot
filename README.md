# FlightReservationBot
Flight Booking Assistant is a prototype application for a chat bot that handles customer reservations.

![alt text](https://flightbot.blob.core.windows.net/container/Robot-Icon.png)

# Features
- guided reservation for flights
- query available destinations
- provide info about extra flight options

# Code behind
The chat bot is implemented using Microsoft Bot Framework. 

The application is capable of recognizing natural language, feature powered by Language Understanding Intelligent Service - *LUIS*.

# Testing the application

## Bot Framework Emulator
1. [Download](https://github.com/Microsoft/BotFramework-Emulator/releases) Bot Framework emulator
2. Install emulator and connect to the REST endpoint of the app (https://localhost:port/api/messages)
3. Install and configure ngrok for connecting to apps hosted remotely
4. You can use either free text form or speech for input

## Bot Framework Developer Portal
1. Publish the app to Azure
2. Connect to [Developer Portal](https://dev.botframework.com/) and register your bot using the endpoint of the app published at step 1.
3. Test using embedded Web Chat and Skype channels.
4. Optionally, configure available alternative channels (Facebook, Slack etc). Set up DirectLine channel for making Web Chat available to custom clients.
