pipeline {
    agent any

    parameters {
        string(name: 'NEXUS_IP', description: 'Private IP of Nexus Server')
    }

    environment {
        APP_NAME = "VowelWebApp"
        VERSION  = "1.0.${BUILD_NUMBER}"
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

        stage('Clean Publish Folder') {
            steps {
                sh 'rm -rf publish'
            }
        }

        stage('Publish') {
            steps {
                sh 'dotnet publish -c Release -o publish'
            }
        }

        stage('Package') {
            steps {
                sh """
                cd publish
                zip -r ../${APP_NAME}-${VERSION}.zip .
                cd ..
                """
            }
        }

        stage('Upload to Nexus') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'nexus-creds',
                    usernameVariable: 'NEXUS_USER',
                    passwordVariable: 'NEXUS_PASS'
                )]) {

                    sh """
                    FILE_NAME=${APP_NAME}-${VERSION}.zip

                    curl -f -u \$NEXUS_USER:\$NEXUS_PASS \
                    --upload-file \$FILE_NAME \
                    http://${params.NEXUS_IP}:8081/repository/dotnet-repo/\$FILE_NAME
                    """
                }
            }
        }
    }

    post {
        success {
            echo "Build and upload successful: ${APP_NAME}-${VERSION}.zip"
        }
        failure {
            echo "Build failed."
        }
    }
}
