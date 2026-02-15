#!/usr/bin/env bash

kafka-topics \
  --bootstrap-server kafka:9092 \
  --create \
  --if-not-exists \
  --topic event-completed \
  --partitions 3 \
  --replication-factor 1

kafka-topics \
  --bootstrap-server kafka:9092 \
  --create \
  --if-not-exists \
  --topic feedback-delayed \
  --partitions 3 \
  --replication-factor 1
