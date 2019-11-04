FROM maheshkvish/bashxampp:latest
WORKDIR /opt/lampp/htdocs/test/
COPY . .
EXPOSE 80