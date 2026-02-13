# Docker Setup Guide

This guide explains how to run the Samma Project using Docker Compose for both development and production environments.

## Prerequisites

- Docker 20.10 or higher
- Docker Compose 2.0 or higher
- At least 4GB of RAM recommended

## Quick Start

### Development Environment

Start the entire application stack in development mode:

```bash
# Start all services (frontend, API, database)
docker-compose up

# Start services in background
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Stop and remove volumes (resets database)
docker-compose down -v
```

### Production Environment

Start the application in production mode:

```bash
# Start with production configuration
docker-compose --profile production up -d

# Or use the production override file
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Stop production services
docker-compose --profile production down
```

## Service Overview

### Development Stack

| Service | Port | Description |
|---------|------|-------------|
| Frontend | 3000 | React development server |
| API | 5001 | .NET backend API |
| PostgreSQL | 5432 | Database |

### Production Stack

| Service | Port | Description |
|---------|------|-------------|
| Frontend | 80 | Nginx serving React app |
| API | 5001 | .NET backend API |
| PostgreSQL | 5432 | Database |

## Development Workflow

### Hot Reload

The development environment supports hot reload for both frontend and backend:

1. **Frontend**: Changes to React components are automatically reflected
2. **Backend**: Code changes trigger automatic restart of the API service

### Database Development

The PostgreSQL database is persistent across container restarts:

```bash
# Access database directly
docker-compose exec postgres psql -U dhamma_user -d dhamma_db

# View database logs
docker-compose logs postgres

# Reset database (WARNING: This will delete all data)
docker-compose down -v && docker-compose up
```

### Environment Variables

Environment variables are configured in:

- **Frontend**: `frontend/.env.development` and `frontend/.env.production`
- **Backend**: Set in `docker-compose.yml` environment section
- **Database**: PostgreSQL environment variables in compose file

## Production Deployment

### Build Images

Build production-ready images:

```bash
# Build all services
docker-compose build

# Build specific service
docker-compose build frontend
docker-compose build api
```

### Deploy to Production

```bash
# Build and start production services
docker-compose --profile production up -d --build

# Verify services are running
docker-compose --profile production ps

# View production logs
docker-compose --profile production logs -f
```

### SSL/HTTPS Setup

For production HTTPS, you'll need to:

1. Update `docker-compose.prod.yml` with your domain
2. Configure SSL certificates
3. Update CORS settings in the API

## Troubleshooting

### Common Issues

**Port Already in Use**
```bash
# Check what's using the port
lsof -i :3000
lsof -i :5001
lsof -i :5432

# Stop conflicting services or change ports in docker-compose.yml
```

**Database Connection Issues**
```bash
# Check if database is ready
docker-compose logs postgres

# Verify connection from API
docker-compose exec api ping postgres

# Reset database if needed
docker-compose down -v
docker-compose up
```

**Frontend Build Issues**
```bash
# Check frontend logs
docker-compose logs frontend

# Rebuild frontend
docker-compose build --no-cache frontend
docker-compose up frontend
```

### Useful Commands

```bash
# View resource usage
docker stats

# Clean up unused images and containers
docker system prune

# View container details
docker inspect <container_name>

# Execute commands in running container
docker-compose exec frontend npm run build
docker-compose exec api dotnet --version
```

## Performance Optimization

### Development

- Volume mounts enable hot reload
- Node modules cached in anonymous volume
- Database data persisted in named volume

### Production

- Multi-stage builds reduce image size
- Nginx serves static files efficiently
- Health checks monitor service status
- Restart policies ensure uptime

## Security Considerations

- Use strong passwords for database
- Enable HTTPS in production
- Regularly update base images
- Use secrets management for sensitive data
- Limit container privileges where possible

## Scaling

### Horizontal Scaling

```bash
# Scale API service
docker-compose up -d --scale api=3

# Scale frontend (if using load balancer)
docker-compose up -d --scale frontend=2
```

### Load Balancing

For production load balancing, consider:

1. Adding a reverse proxy (nginx, traefik)
2. Using Docker Swarm or Kubernetes
3. Implementing service discovery

## Monitoring

### Health Checks

Services include health checks:

- Frontend: HTTP check on port 3000/80
- API: HTTP check on port 8080
- Database: PostgreSQL connection check

### Logging

Centralized logging with Docker:

```bash
# View all logs
docker-compose logs

# Follow logs in real-time
docker-compose logs -f

# Filter logs by service
docker-compose logs frontend
docker-compose logs api
```

## Backup and Recovery

### Database Backup

```bash
# Backup database
docker-compose exec postgres pg_dump -U dhamma_user dhamma_db > backup.sql

# Restore database
docker-compose exec -T postgres psql -U dhamma_user -d dhamma_db < backup.sql
```

### Volume Backup

```bash
# Backup database volume
docker run --rm -v dhamma-postgres-data:/volume -v $(pwd):/backup alpine tar czf /backup/postgres-backup.tar.gz -C /volume .

# Restore volume
docker run --rm -v dhamma-postgres-data:/volume -v $(pwd):/backup alpine tar xzf /backup/postgres-backup.tar.gz -C /
```

## Development Tips

1. **Use `.dockerignore`**: Exclude unnecessary files from build context
2. **Layer Caching**: Order Dockerfile instructions to maximize cache hits
3. **Volume Permissions**: Ensure proper file permissions for mounted volumes
4. **Network Isolation**: Use custom networks for service isolation
5. **Resource Limits**: Set memory and CPU limits in production

## Next Steps

1. Configure CI/CD pipeline for automated builds
2. Set up monitoring and alerting
3. Implement backup automation
4. Configure SSL certificates
5. Set up staging environment