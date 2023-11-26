## What is this

This is a test solution that demonstrates hotel room booking between 2 docker containers. The user can access client front-end application to do following:

* View customer's future reservations
* See available rooms
* Book a free room
* Cancel an existing reservation provided it's less than 3 days before the start

There's also backoffice view that can be used by employees. The employee can

* Change reservation start/end dates
* Cancel an existing reservation without restriction
* View all reservations

In order to keep the scope managable, different features that would exist in real-life applications have been omitted (for example, customer and employee authentication/authorization).


## How to run

### Requirements

Tooling requirements to build and run solution are:

* [Docker](https://docs.docker.com/engine/install/).
* Make - optional (can be installed with chocolatey or https://gnuwin32.sourceforge.net/packages/make.htm)

The docker-compose.yml is configured to use ports 8080 and 5173. If there's a need to change the ports, please update the docker-compose.yml.

### Commands

It is recommended to run this solution using docker-compose file supplied, this ensures applications work properly with not additional fine tuning required.

If you have make installed you can use following commands:

* make build-all
* make run
* make stop

If you don't have make installed, you can refer to contents of the "Makefile" and run the commands manually

### General information

If you're using the default configurations, frontend should be accessible from http://localhost:5173

There is also some database seeding implemented. If there's a need for more data, the Booking.Server/Program.cs can be updated with needed data.
