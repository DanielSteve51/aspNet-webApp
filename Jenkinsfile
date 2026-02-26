pipeline {
    agent any

    environment {
        APP_NAME = "VowelWebApp"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Publish') {
            steps {
                sh 'dotnet publish -c Release -o publish'
            }
        }

        stage('Package') {
            steps {
                sh 'zip -r ${APP_NAME}.zip publish/'
            }
        }
    }
}
