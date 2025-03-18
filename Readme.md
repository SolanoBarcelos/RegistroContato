# 📌 RegistroContato - Documentação

## 🎥 Demonstração

Para ver o projeto em funcionamento, assista ao vídeo abaixo:

[![Demonstração do Projeto](https://img.youtube.com/vi/1KZQEo6E_FdMXX_yOTLND7-t9AbpdWNKX/hqdefault.jpg)](https://drive.google.com/file/d/1KZQEo6E_FdMXX_yOTLND7-t9AbpdWNKX/view?usp=sharing "Clique para assistir")

## 🛠️ Configuração e Inicialização do Projeto

Este projeto utiliza **Docker** e **Kubernetes** para a gestão dos containers. Ele inclui:

- **APIs REST**
- **Mensageria RabbitMQ** (Consumers e Producers)
- **Banco de Dados PostgreSQL**
- **Monitoramento com Grafana & Prometheus**

## ✅ **Pré-requisitos**

1. **Docker** e **kubectl** instalados
2. **Acesso ao repositório**
3. **Acesso ao Docker Hub** para baixar as imagens
4. **Variáveis de ambiente configuradas** (consulte o .env.example).

## 📥 **Clonando o Repositório**

```sh
git clone https://github.com/SolanoBarcelos/RegistroContato.git
cd RegistroContato
```

---

## 🚀 **Rodando com Docker Compose**

### \*\*1️⃣ Criar o arquivo \*\***`.env`**

```sh
cp .env.example .env
nano .env  # Edite conforme necessário
```

### **2️⃣ Subir os Containers**

```sh
docker compose up --build -d
```

- `--build`: Garante que as imagens serão reconstruídas
- `-d`: Roda os containers em **modo detach** (segundo plano)

📌 **Se precisar reiniciar tudo do zero:**

```sh
docker compose down -v && docker compose up --build -d
```

---

## ⚡ **Rodando no Kubernetes**

### **1️⃣ Aplicar Configurações**

```sh
kubectl apply -f kubernetes_deployments.yaml
kubectl apply -f kubernetes_db.yaml
kubectl apply -f kubernetes_rabbitmq.yml
kubectl apply -f prometheus-config.yaml
kubectl apply -f kind-config.yaml
kubectl apply -f secrets.yaml
kubectl apply -f configmap.yamlaml

```

### **2️⃣ Verificar os Pods**

```sh
kubectl get pods -n registro-contato
```

📌 Se precisar deletar e recriar os Pods:

```sh
kubectl delete pods --all -n registro-contato
kubectl apply -f kubernetes_deployments.yaml
kubectl apply -f kubernetes_db.yaml
kubectl apply -f kubernetes_rabbitmq.yaml
```

---

## 🎯 **Acessando os Serviços**

### 🗄️ **Banco de Dados (PostgreSQL)**

- **Host:** `db-contato.registro-contato.svc.cluster.local`
- **Porta:** `5432`
- **Usuário:** `${DB_USER}`
- **Senha:** `${DB_PASS}`
- **Nome do Banco:** `${DB_NAME}`

📌 **Acesso via terminal:**

```sh
kubectl exec -it db-contato -- psql -U ${DB_USER} -d ${DB_NAME} -n registro-contato
```

📌 **Acesso via DBeaver:**

1. Adicionar nova conexão PostgreSQL
2. **Host:** `localhost` (ou o NodePort)
3. **Porta:** `5432`
4. **Usuário:** `${DB_USER}`
5. **Senha:** `${DB_PASS}`

Para expor a porta do banco:

```sh
kubectl port-forward svc/db-contato 5432:5432 -n registro-contato
```

---

### 📊 **Monitoramento (Grafana & Prometheus)**

#### 🔍 **Prometheus** *(coleta métricas)*

- **URL:** `http://<NodeIP>:31623`  *(NodePort)*
- **Fonte de dados para Grafana:** `http://prometheus:9090`

#### 📈 **Grafana** *(visualização de métricas)*

- **URL:** `http://<NodeIP>:3000`
- **Usuário:** `admin`
- **Senha:** `${GF_SECURITY_ADMIN_PASSWORD}` *(definido no **************`.env`**************)*

📌 **Para salvar dashboards:**

- Adicione um **Persistent Volume** ao Grafana.
- Configure um `ConfigMap` para salvar as configurações.

---

### 📡 **Mensageria (RabbitMQ)**

- **URL:** `http://<NodeIP>:15672`
- **Usuário:** `${RABBITMQ_USER}`
- **Senha:** `${RABBITMQ_PASS}`

📌 **Acesso via terminal:**

```sh
kubectl exec -it rabbitmq -n registro-contato -- rabbitmqctl list_queues
```

Se precisar expor a porta:

```sh
kubectl port-forward svc/rabbitmq 15672:15672 -n registro-contato
```

---

## 🧪 **Executando os Testes**

📌 **Rodar testes manualmente:**

```sh
docker compose up --build tests-unit tests-integration
```

📌 **Separando por categoria:**

```sh
docker compose run --rm tests-unit
docker compose run --rm tests-integration
```

🚀 **Os resultados ficam armazenados em ****************************`TestResults/`**************************** dentro do container.**

---

## 🔄 **Parar e Remover Containers**

```sh
docker compose down -v
```

---

## 🚀 **CI/CD com GitHub Actions**

O repositório inclui um **workflow automatizado no GitHub Actions** para rodar os testes sempre que um commit é enviado.

- **Pipeline roda automaticamente nos PRs e commits na**  ```master e na develop```
- **Se falhar nos testes, o deploy não é realizado.**

Caso precise modificar o workflow, edite o arquivo:

```sh
.github/workflows/ci.yml
```

---

## 🔗 **Referências**

- [Documentação do Docker](https://docs.docker.com/)
- [Documentação do Grafana](https://grafana.com/docs/)
- [Documentação do Prometheus](https://prometheus.io/docs/)
- [Documentação do RabbitMQ](https://www.rabbitmq.com/documentation.html)
- [Documentação do PostgreSQL](https://www.postgresql.org/docs/)

