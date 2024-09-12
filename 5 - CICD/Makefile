all: clean build exec

clean:
	@echo "Cleaning Targets..."
	@mvn clean
build:
	@echo "Building Artifacts"
	@mvn compile
exec:
	@echo "Executing Application"
	@mvn exec:java
test:
	@echo "Building/Running Tests"
	@mvn test