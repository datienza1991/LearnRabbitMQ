# user/pass: guest
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.11-management

# remove containers
docker rm -f  $(docker ps -q)

# create docker image with rabbitmq plugins
docker build -t rabbitmq-11-management .

# user/pass: guest from build image docker file
docker run -it --rm -p 5672:5672 -p 15672:15672 rabbitmq-11-management