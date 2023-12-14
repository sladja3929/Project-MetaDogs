#set base image
FROM python:3.8.1

#set working directory
WORKDIR /src

#copy host files to working dir
COPY src/ .

#install dependencies
RUN pip install -r requirements.txt

#command to run on container start
CMD [ "python3", "./app.py" ]