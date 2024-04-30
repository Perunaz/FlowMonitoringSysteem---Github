# Use an official Node runtime as a parent image
FROM node:21-alpine3.18

# Set the working directory to /app
WORKDIR /app

# Copy the current directory contents into the container at /app,
# excluding items specified in .dockerignore
COPY . /app

# Install app dependencies
RUN npm install --production

EXPOSE 7071

# Start the app (as defined in package.json)
CMD [ "npm", "run", "start" ]