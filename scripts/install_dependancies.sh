echo "installing hacking software..."
sleep 1
# installation procedure
ubuntu_release=$(lsb_release -d | sed 's/.*Ubuntu \(\)/\1/g')

if [[ ${ubuntu_release} == *20.04* ]]; then
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb

    sudo apt-get update && \
    sudo apt-get install -y dotnet-sdk-6.0
    sudo apt-get update && \
    sudo apt-get install -y aspnetcore-runtime-6.0
elif [[ ${ubuntu_release} == *22.04* ]]; then
    sleep 3
    
    sudo apt update
    sudo apt install -y dotnet-sdk-6.0
    sudo apt-get install -y dotnet-runtime-6.0
 
else
    echo "ERROR: UNSUPPORTED DISTRIBUTION OF LINUX"
    echo "Supported versions are Ubuntu 20.04 LTS and Ubuntu 22.04 LTS"
    sleep 5
fi