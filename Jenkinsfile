    pipeline {
        agent any

        parameters {
            string(name: 'NEXUS_IP', description: 'Private IP of Nexus Server')
            string(name: 'DEPLOY_IP', description: 'Private IP of DOtNet Deploy Server')
            string(name: 'REPO_NAME', description: 'Nexus Repository Name (Example: dotnet-repo)')
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

            stage('Sonar Begin') {
                steps {
                    withSonarQubeEnv('sonar-server') {
                        withCredentials([string(credentialsId: 'sonar-cred', variable: 'SONAR_TOKEN')]) {
                            sh """
                            dotnet sonarscanner begin \
                            /k:"$APP_NAME" \
                            /d:sonar.host.url=$SONAR_HOST_URL \
                            /d:sonar.login=$SONAR_TOKEN
                            """
                        }
                    }
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

            stage('Sonar End') {
                steps {
                    withSonarQubeEnv('sonar-server') {
                        withCredentials([string(credentialsId: 'sonar-cred', variable: 'SONAR_TOKEN')]) {
                            sh """
                            dotnet sonarscanner end \
                            /d:sonar.login=$SONAR_TOKEN
                            """
                        }
                    }
                }
            }

            stage("Quality Gate") {
                steps {
                    timeout(time: 2, unit: 'MINUTES') {
                        waitForQualityGate abortPipeline: true
                    }
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
                build job: 'aspNet_webApp_CD_TEST',
            parameters: [
                string(name: 'NEXUS_IP', value: params.NEXUS_IP),
                string(name: 'DEPLOY_IP', value: params.DEPLOY_IP),
                string(name: 'APP_NAME', value: APP_NAME),
                string(name: 'REPO_NAME', value: 'dotnet-repo'),
                string(name: 'VERSION', value: VERSION)
            ]
            }
            failure {
                echo "Build failed."
            }
        }
    }
