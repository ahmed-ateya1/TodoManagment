# Use official Nginx image
FROM nginx:alpine

# Remove default nginx content
RUN rm -rf /usr/share/nginx/html/*

# Copy only static site files (index.html, CSS, js folders)
COPY index.html /usr/share/nginx/html/index.html
COPY CSS /usr/share/nginx/html/CSS
COPY js /usr/share/nginx/html/js

# Expose default Nginx port
EXPOSE 80
