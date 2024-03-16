FROM postgres:16.2-alpine3.19

# set env variable in order for a non-interactive update process
ENV DEBIAN_FRONTEND=noninteractive

# update packages
RUN apk update && apk upgrade
# create new user with lower privileges
RUN adduser -D cervantes -G postgres
# change permissions of particular directories
RUN chown -R cervantes /var/lib/postgresql /var/run/postgresql

# switch to the newly created user
USER cervantes
