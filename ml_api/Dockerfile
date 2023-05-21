
#set base image
FROM python:3.9.9

#set mlagent working directory
WORKDIR /mlagent
COPY mlagent/ .

#install mlagent dependencies
RUN python -m pip install pip==21.2.4
RUN pip intall -r requirements.txt

RUN git clone --branch release_20 https://github.com/Unity-Technologies/ml-agents.git
RUN pip install -e ml-agents/ml-agents-envs/
RUN pip install -e ml-agents/ml-agents/

RUN git clone --branch server_AI https://github.com/sladja3929/Project-MetaDogs.git
RUN chmod -R 755 Project-MetaDogs/Linux-build/AITest.x86_64
RUN chmod -R 755 Project-MetaDogs/Linux-build/UnityPlayer.so

#set api working directory
WORKDIR /src
COPY src/ .

#install api dependencies
RUN pip install -r requirements.txt

CMD [ "python3", "./app.py" ]
