FROM nginx:1.25.4-alpine3.18

# set env variable in order for a non-interactive update process
ENV DEBIAN_FRONTEND=noninteractive

# update packages
RUN apk update && apk upgrade
# create new user with lower privileges
RUN adduser -D cervantes
# change permissions of particular directories
RUN chown -R cervantes /var/cache/nginx /docker-entrypoint.d /etc/nginx
RUN chown cervantes /var/run

# switch to the newly created user
USER cervantes
