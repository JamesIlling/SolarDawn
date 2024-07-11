# 2. Implement as containers

Date: 2024-07-11

## Status

Accepted

## Context

When we write code for this system we may want to deploy it to many different environments, the currently expected list is

* Windows 11 64-bit PC
* Raspberry Pi 4.0 Arm-64bit
* AWS.

We also want to maintain each components separately and be able to easily add new items into the system.

## Decision

We will implement each component as a docker container, these will be built for:

* Linux
* Arm

Windows machines are capable of running Linux containers using Docker, so this meets our current needs.

## Consequence

* Code isolation becomes simple with communication between components becoming web based calls.
* Deployment of the system becomes more platform independent
* Cross compilation of executables may be needed for arm based systems.
* This allows a single physical computer to host some or all of the system.
