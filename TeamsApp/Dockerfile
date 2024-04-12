# Use an official Node runtime as a parent image
FROM node:21-alpine3.18

# Set the working directory to /app
WORKDIR /app

# Copy the current directory contents into the container at /app
COPY . /app

# Install app dependencies
RUN npm install --production

# Build and optimize react app
RUN npm run build

EXPOSE 53000

# defined in package.json
CMD [ "npm", "run", "dev:teamsfx" ]