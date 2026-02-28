pipeline {
    agent any

    parameters{
        string(name:'NEXUS_IP', description:'Private IP of Nexus Server')
    }
    
    environment {
        APP_NAME = "VowelWebApp"
        VERSION = "1.0.${BUILD_NUMBER}"
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
                sh 'zip -r ${APP_NAME}-${VERSION}.zip publish/'
            }
        }

       stage('Upload to Nexus') {
    steps {
        sh """
        FILE_NAME=${APP_NAME}-${VERSION}.zip
        curl -v -u USER:PASS \
        --upload-file \$FILE_NAME \
        http://${params.NEXUS_IP}:8081/repository/dotnet-repo/\$FILE_NAME
        """
    }
}
    }
}
