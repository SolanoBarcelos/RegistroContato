# 📌 RegistroContato - Documentação

## 🛠️ Configuração e Inicialização do Projeto

Este projeto é containerizado usando **Docker** e gerenciado pelo **Docker Compose**. Ele inclui diversas integrações como **RabbitMQ, PostgreSQL, Grafana, Prometheus** e **Node Exporter** para monitoramento.

### ✅ **Pré-requisitos**

1. **Docker e Docker Compose** instalados.
2. **Variáveis de ambiente configuradas** (consulte o `.env.example`).
3. **Acesso ao repositório** para clonar o projeto.

### 📥 **Clonando o repositório**

```sh
git clone https://github.com/SolanoBarcelos/RegistroContato.git
cd RegistroContato
```

### 🛠️ **Configuração do ambiente**

Crie um arquivo `.env` na raiz do projeto e preencha com os valores necessários.
Veja o exemplo no `.env.example`.

```sh
cp .env.example .env
nano .env # Edite conforme necessário
```

### 🚀 **Construindo e Subindo os Containers**

```sh
docker-compose up --build -d
```

- `--build`: Garante que as imagens serão reconstruídas caso necessário.
- `-d`: Roda os containers em **modo detach** (em segundo plano).

Caso queira **reiniciar do zero**:

```sh
docker-compose down -v && docker-compose up --build -d
```

---

## 🎯 **Acessando os Serviços**

### 🗄️ **Banco de Dados (PostgreSQL)**

- **Host:** `${DB_HOST}` *(definido no ******`.env`******)*
- **Porta:** `${DB_PORT}` *(padrão: 5432)*
- **Usuário:** `${DB_USER}`
- **Senha:** `${DB_PASS}`
- **Nome do Banco:** `${DB_NAME}`

📌 **Acesso via terminal:**

```sh
docker exec -it db_contato psql -U ${DB_USER} -d ${DB_NAME}
```

📌 **Acesso via pgAdmin** *(se instalado localmente)*:

- **URL:** `http://localhost:5050`
- **Usuário/Senha**: Configurados via `.env`

📌 **Importante:**

- **O banco é criado automaticamente pelo Docker**, não é necessário configurar nada manualmente.
- **Durante os testes, a tabela ************`ContatosTestes`************ é criada automaticamente e truncada a cada execução.**

---

### 📊 **Monitoramento (Grafana & Prometheus)**

#### 🔍 **Prometheus** *(coleta métricas)*

- **URL:** [http://localhost:9090](http://localhost:9090)
- \*\*Configuração automática via \*\***`prometheus.yml`**

#### 📈 **Grafana** *(visualização de métricas)*

- **URL:** [http://localhost:3000](http://localhost:3000)
- **Usuário:** `admin`
- **Senha:** `${GRAFANA_ADMIN_PASSWORD}` *(definido no ******`.env`******)*

📌 **Passo inicial**: Após o login no Grafana, configure a fonte de dados como **Prometheus (********`http://prometheus:9090`********\*\*\*\*)**.

---

### 📡 **Mensageria (RabbitMQ)**

- **URL:** [http://localhost:15672](http://localhost:15672) *(Painel de Gerenciamento)*
- **Usuário:** `${RABBITMQ_USER}`
- **Senha:** `${RABBITMQ_PASS}`

📌 **Acesso via terminal:**

```sh
docker exec -it rabbitmq rabbitmqctl list_queues
```

---

## 🧪 **Executando os Testes**

O projeto está configurado para rodar testes de **Unidade** e **Integração** separadamente usando **xUnit**.

📌 **Os testes criam e utilizam a tabela ************`ContatosTestes`************, que é truncada a cada execução.**

### **Rodar Testes Manualmente**

```sh
docker-compose up --build tests-unit tests-integration
```

📌 **Separando por categoria:**

```sh
docker-compose run --rm tests-unit
```

```sh
docker-compose run --rm tests-integration
```

🚀 **Os resultados ficam armazenados em** `TestResults/` dentro do container.

---

## 🔄 **Parar e Remover Containers**

```sh
docker-compose down -v
```

- `-v`: Remove volumes persistentes para garantir uma reinicialização limpa.

---

## 🚀 **CI/CD com GitHub Actions**

O repositório inclui um **workflow automatizado no GitHub Actions** para rodar os testes toda vez que um commit é enviado.

- **Pipeline roda automaticamente nos PRs e commits na ************`main`************.**
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

