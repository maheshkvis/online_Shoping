FROM maheshkvish/xamppbash:latest
WORKDIR /opt/lampp/htdocs/
RUN rm -r test
RUN mkdir test
WORKDIR /opt/lampp/htdocs/test/
COPY . .
EXPOSE 80