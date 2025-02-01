FROM pgvector/pgvector:pg17

# set env variable in order for a non-interactive update process
ENV DEBIAN_FRONTEND=noninteractive

# update packages
RUN apt update
RUN apt upgrade -y
# create new user with lower privileges
RUN useradd -m -g postgres cervantes
# change permissions of particular directories
RUN chown -R cervantes /var/lib/postgresql /var/run/postgresql

# switch to the newly created user
USER cervantes
