echo "uninstalling software..."

# removal procedure
ubuntu_release=$(lsb_release -d | sed 's/.*Ubuntu \(\)/\1/g')

if [[ ${ubuntu_release} == *20.04* ]]; then
    sudo dpkg -r packages-microsoft-prod.deb

    sudo apt-get update && \
    sudo apt-get remove -y dotnet-sdk-6.0
    sudo apt-get update && \
    sudo apt-get remove -y aspnetcore-runtime-6.0
elif [[ ${ubuntu_release} == *22.04* ]]; then
    sleep 3
    
    sudo apt update
    sudo apt remove -y dotnet-sdk-6.0
    sudo apt-get remove -y dotnet-runtime-6.0
else
then
    echo "ERROR: UNSUPPORTED DISTRIBUTION OF LINUX"
    echo "Supported versions are Ubuntu 20.04 LTS and Ubuntu 22.04 LTS"
    sleep 5
    exit
fi

echo "Successfully removed software"