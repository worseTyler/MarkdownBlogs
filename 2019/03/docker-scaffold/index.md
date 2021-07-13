

## 

 "Effortlessly Scaffold Your App with Docker - Part 1"

Dockerize your next ASP.NET Core, Postgres and Angular app to easily get up and running.

### **Abstract**

In this tutorial, you'll learn how to use [Docker](https://www.docker.com) to set up a fresh [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/) project connected to a [PostgreSQL](https://www.postgresql.org/) database. As we'll find, Docker will take care of all the heavy lifting when it comes to installing these packages and coordinating their execution. In fact, it's so easy to develop using containers, we'll throw in an [Angular](https://angular.io/) app just because we can! By the end, we'll have a full web-app framework in place, replete with a client, REST API and database.

### **Show me the code**

Go and get the final project template [here](https://github.com/the-fool/Dotnet-Postgres-Docker).

To run the app, first make sure Docker and [Docker-Compose](https://docs.docker.com/compose/) are installed on your machine, and then:

```
git clone https://github.com/the-fool/dotnet-postgres-docker
cd dotnet-postgres-docker
docker-compose up
```

That's it! After a few minutes, you'll be able to visit the Angular app at `localhost:4200` and the REST API at `localhost:5000`.

Read on to understand how this was built up from scratch.  We’ll step through a Docker-oriented workflow with each piece of the app and glean some useful patterns for using containers during development. This crash-course in full-stack web development will serve as a helpful case study for hooking up an ASP.NET to a non-Microsoft database, building out a minimal REST API to be consumed by an Angular client and using Docker to hold it all together.

### **Application structure**

We've been tasked to develop a web app for the international retail juggernaut **Gadget Depot**.

- The CTO has one requirement: display Gadget Depot's current inventory.
- The budget is $50,000.
- It needs to be done in **30 minutes**.

Good thing we're standing on the shoulders of containerized giants. We're going to accomplish this with a .NET Core Web API, backed by PostgreSQL and consumed by an Angular client.

Start the timer, and let's code!

Pick a spot in your file system, and make the root project directory.

```
mkdir gadget\_depot
cd gadget\_depot
```

It'll be nice to keep the server code and client code entirely separate. The backend will be _merely_ an API service. No C# code will be responsible for presentation. All the UI code will be in its own module, set up to consume our API.

To indicate the independence of the frontend and backend, make two sub-directories in the root of the project.

```
\# at the project root
mkdir Frontend
mkdir Backend
```

Now we need to scaffold out the boilerplate code for both of our projects. Nothing stops us from writing it all by hand, following the [completed code](https://github.com/the-fool/dotnet-postgres-docker) as a guide. However, Microsoft and Angular each provide tools for generating starter-templates. We'll use those tools ([.NET Core CLI](https://docs.microsoft.com/dotnet/core/tools/) and \[[Angular CLI](https://cli.angular.io/)) to save us some time and tedium.

### **Scaffold .NET Core backend**

Let's scaffold the backend first, using the `dotnet` program.

```
\# at the project root
cd Backend
# create a new solution
docker run -v $(pwd):/app -w /app microsoft/dotnet dotnet new sln -n gadget\_depot
# create the webapi project
mkdir GadgetDepot
docker run -v $(pwd):/app -w /app microsoft/dotnet dotnet new webapi -o GadgetDepot
# add the project to the solution
docker run -v $(pwd):/app -w /app microsoft/dotnet dotnet sln add GadgetDepot
```

In the `backend` directory, you should have a file tree resembling the following:

│   GadgetDepot.sln
│   
└───GadgetDepot
    │   appsettings.Development.json
    │   appsettings.json
    │   GagdetDepot.csproj
    │   Program.cs
    │   Startup.cs
    │
    ├───bin
    │
    ├───Controllers
    │
    ├───Models
    │
    ├───obj
    │
    └───Properties

Notice that we used a dockerized `dotnet` executable. Maybe you already had the `dotnet` program installed on your OS -- could you just use that?  Perhaps. One concern is _which version_ of `dotnet` are you running. And, if you update your system-wide `dotnet` for this project, would you then break your SDK for existing projects in your environment? These are tough questions.

Docker to the rescue!

We were able to scaffold all this code through without needing to worry about the specifics of the host machine we’re using to build this app.

### **Scaffold Angular frontend**

No surprise: We can also leverage Docker for creating our Angular app!

Go back to the root of our project, and on into the frontend directory.

```
\# go back to root from within Backend
cd ..
cd Frontend
```

To generate code for a simple Angular app, the command to run is `ng new gadgets --minimal --directory ./`. Without needing to install the `ng` program, we're going to use a Docker image that contains the [Angular CLI tool](https://cli.angular.io/).

```
docker run -v $(pwd):/app -w /app johnpapa/angular-cli ng new gadgets --minimal --directory ./
```

This command readies an app template and installs a boatload of NodeJS modules. After a few minutes of installation, you should have a fully-loaded and operational Angular app.

The last step is to arrange our separate app components so that they boot up in such a way that they can network with each other.

### **Docker-Compose enters the ring**

To orchestrate multiple containers, we'll use [Docker Compose](https://docs.docker.com/compose/). It's a handy tool for configuring your containerized apps to work together.

_We're moving fast. If you haven't seen Docker-Compose, or Docker, there are a plethora of useful_ _tutorials_ _out on the net. Try [this one](https://docs.docker.com/get-started/) or [this one](https://docker-curriculum.com/). If need be, go acquaint yourself then come back – otherwise, we can march on._

In the root of the project, create a file `docker-compose.yml`.

```
\# /docker-compose.yml
version: "3"

volumes:
  local\_postgres\_data: {}

services:
  web:
    build: ./Backend
    ports:
      - "5000:5000"
      - "5001:5001"
    volumes:
      - ./backend:/app
      - /app/GadgetDepot/bin
      - /app/GadgetDepot/obj
    depends\_on:
      - db

  db:
    image: postgres:11.1
    environment:
      POSTGRES\_PASSWORD: postgres
      POSTGRES\_USERNAME: postgres
    volumes:
      - local\_postgres\_data:/var/lib/postgresql/data

  client:
    build: ./Frontend
    ports:
      - "4200:4200"
    volumes:
      - ./frontend:/app
```

In this file, we declare our three separate _services_ comprising the app.

-     `web` : the .NET Core Web API project
-     `db` : the database
-     `client` : the Angular app

One piece especially worth pointing out is the `local_postgres_data` volume. By declaring a "volume," we can _persist_ our database state beyond the lifetime of the `db` container. The call to create a volume allocates space on the host OS which outlives the destruction of a container. When we reboot our PostgreSQL service, the database will have retained all its tables and rows, ready to go as if nothing had happened.  If we didn't map the container's `/var/lib/postgresql/data` dir to our host filesystem, the container would boot with fresh state when created. In some cases, you might want this behavior! But for development, it’s convenient to persist some of your data.

Notice that `web` and `client` services specify a `build` property. This property tells Docker where to look for a `Dockerfile` it can use to build the containers. Right now, it wouldn't find one since we haven't created it. So let's add a `Dockerfile` to each the `./Frontend` and `./Backend` directories.

**For the backend:**

```
\# ./Backend/Dockerfile
FROM microsoft/dotnet:latest

COPY ./entrypoint.sh /
RUN sed -i 's/\\r//' /entrypoint.sh
RUN chmod +x /entrypoint.sh

WORKDIR /app

CMD /entrypoint.sh
```

**For the frontend:**

```
\# ./Frontend/Dockerfile
FROM node:latest

COPY ./entrypoint.sh /
RUN sed -i 's/\\r//' /entrypoint.sh
RUN chmod +x /entrypoint.sh

WORKDIR /app

CMD /entrypoint.sh
```

Each of these are very similar and straightforward. After specifying a base image, most of their steps pertain to an `entrypoint.sh` script, which will get run by default when the container starts. We need to provide this shell script.

### **Write the Startup scripts**

For the last bit of Docker plumbing, we need to write an entry script for each of our services. This script acts as the 'bootup' command for the containers. The frontend and the backend scripts resemble each other closely - they each install dependencies and start a dev server.

For `./Backend/entrypoint.sh`

```
#!/bin/bash
set -e
dotnet restore
# test the DB connection
until dotnet ef -s GadgetDepot -p GadgetDepot database update; do
>&2 echo "DB is starting up"
sleep 1
done
>&2 echo "DB is up - executing command"
dotnet watch -p GadgetDepot run
```

For `./Frontend/entrypoint.sh`

```
#!/bin/bash
set -e
yarn
npm start
```

The .NET script restores its packages, updates the database, then runs a server in dev mode. The Angular script installs packages and boots up a dev server.

Easy as pie!

Now, for the grand finale, we can boot up our whole, orchestrated app with a single command in the root directory:

```
\# in project root
docker-compose up
```

With one line, all the containers will build and configure themselves, ready to display the inventory listing for Gadget Depot! Get ready to cash that check!

_Well – not quite!_

In the [next blog](https://intellitect.com/docker-postgresql/), we'll add Postgres to our application and build out our API code.

_Written by Thomas Ruble._
