# Use an official Node runtime as a parent image
FROM node:21-alpine3.18

# Set the working directory to /app
WORKDIR /app

# Copy the current directory contents into the container at /app
COPY . /app

# Install any needed packages specified in package.json
RUN npm install --production

# Install type definitions
RUN npm install --save-dev @types/express
RUN npm install --save-dev @types/amqplib
RUN npm install --save-dev @types/cors

# Make port 3100 available to the world outside this container
EXPOSE 3100

# Run the command to start the app
CMD ["npm", "start"]
