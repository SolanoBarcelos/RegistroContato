Como rodar o projeto em Containers no Docker

DOCKER
NETWORK
# Criar rede para os containers compartilharem dados
docker network create --driver bridge postech


POSTGRES
docker pull postgres
docker run -d --name db_contato --network postech -p 5432:5432 -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=1234 -e POSTGRES_DB=db_contato postgres:latest

criar tabela

	CREATE TABLE contatos (
    id_contato INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome_contato VARCHAR(100) NOT NULL,
    telefone_contato VARCHAR(50) NOT NULL,
    email_contato VARCHAR(100) NOT NULL
);

APLICAÇÂO
# Criar Dockerfile na pasta raiz, rodar comando de build na pasta raiz
docker build -t sfbarcelos/cadastro_persistencia_image .
docker run -d --name cadastro_persistencia_container --network postech -p 7070:7070 sfbarcelos/cadastro_persistencia_image:latest

NODE EXPORTER
# Monitorar CPU e Memoria 
docker pull prom/node-exporter
docker run -d -p 9100:9100 --name node_exporter_container --network postech --restart unless-stopped prom/node-exporter


PROMETHEUS
docker pull prom/prometheus
# criar uma pasta com nome "Prometheus_custom", dentro dela criar os arquivos promethus.yml e Docker file, rodar a Docker build na pasta do prometheus com os arquivos dockerfile e prometheus.yml. Substituir pelo nome da imagem pelo seu namespace no Docker hub. Veja os arquivos no próximo tópico.
-- docker build -t sfbarcelos/prometheus_image:latest .
docker run -d --name prometheus_agent_container --network postech -p 9090:9090 sfbarcelos/prometheus_image:latest -- substituir pela pelo nome da imagem que você tagueou

ARQUIVOS PROMETHEUS
- Dockerfile - Não tem extenção

FROM prom/prometheus
COPY prometheus.yml /etc/prometheus/prometheus.yml

- promethus.yml

global:
  scrape_interval: 5s

global:
  scrape_interval: 5s

scrape_configs:
  - job_name: "addcontatoconsumer"
    scrape_interval: 5s
    static_configs:
      - targets: ["host.docker.internal:7071"]

  - job_name: "Addcontatoconsumer"
    scrape_interval: 5s
    static_configs:
      - targets: ["host.docker.internal:7072"]

  - job_name: "deletecontato"
    scrape_interval: 5s
    static_configs:
      - targets: ["host.docker.internal:7073"]

  - job_name: "getcontato"
    scrape_interval: 5s
    static_configs:
      - targets: ["host.docker.internal:7074"]

  - job_name: "updatecontatoconsumer"
    scrape_interval: 5s
    static_configs:
      - targets: ["host.docker.internal:7075"]

  - job_name: "updatecontatoconsumer"
    scrape_interval: 5s
    static_configs:
      - targets: ["host.docker.internal:7076"]

  - job_name: "node_exporter"
    static_configs:
      - targets: ["node_exporter_container:9100"]


GRAFANA
docker pull grafana/grafana
-- docker tag grafana/grafana:latest sfbarcelos/grafana_image:latest -- -- se quiser taguear a image
docker run -d -p 3000:3000 --name grafana_monitoring_container --network postech grafana/grafana:latest -- substituir pela pelo nome da imagem que você tagueou


RABBITMQ
docker run --network postech --name rabbitmq_masstransit_container -p 15672:15672 -p 5672:5672 masstransit/rabbitmq
