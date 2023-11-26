.phony: build-client
build-client:
	docker build -t booking-client:latest -f ./booking.client/Dockerfile ./booking.client

.phony: build-server
build-server:
	docker build -t booking-server:latest -f ./Booking.Server/Dockerfile ./Booking.Server

.phony: build-all
build-all: build-client build-server
	

.phony: run
run:
	docker compose up -d

.phony: stop
stop:
	docker compose down