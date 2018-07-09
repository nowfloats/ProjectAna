#!/bin/bash
if [ "$TRAVIS_BRANCH" == "release" ]; then
    # Linux upload
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then which python; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then pip install awscli --upgrade --user; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then ls -al /usr/local/bin/python; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then export PATH=~/.local/bin:$PATH; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then source ~/.bash_profile; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then aws s3 cp *.AppImage s3://ana-content/cdn/dist/ana-app/linux-ia32/ --acl public-read; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then aws s3 cp *.AppImage s3://ana-content/cdn/dist/ana-app/linux-x64/ --acl public-read; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then aws s3 cp latest-linux.yml s3://ana-content/cdn/dist/ana-app/linux-ia32/ --acl public-read; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then aws s3 cp latest-linux.yml s3://ana-content/cdn/dist/ana-app/linux-x64/ --acl public-read; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then aws s3 ls s3://ana-content/cdn/dist/ana-app/linux-ia32/ --human-readable --summarize; fi
    if [ "$TRAVIS_OS_NAME" == "linux" ]; then aws s3 ls s3://ana-content/cdn/dist/ana-app/linux-x64/ --human-readable --summarize; fi
    # Mac only Upload to s3
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then aws --version; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then which python; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then ls -al /usr/local/bin/python; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then export PATH=~/.local/bin:$PATH; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then source ~/.bash_profile; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then aws s3 cp *.dmg s3://ana-content/cdn/dist/ana-app/mac-x64/ --acl public-read; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then aws s3 cp *.dmg.blockmap s3://ana-content/cdn/dist/ana-app/mac-x64/ --acl public-read; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then aws s3 cp *.zip s3://ana-content/cdn/dist/ana-app/mac-x64/ --acl public-read; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then aws s3 cp latest-mac.json s3://ana-content/cdn/dist/ana-app/mac-x64/ --acl public-read; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then aws s3 cp latest-mac.yml s3://ana-content/cdn/dist/ana-app/mac-x64/ --acl public-read; fi
    if [ "$TRAVIS_OS_NAME" == "osx" ]; then aws s3 ls s3://ana-content/cdn/dist/ana-app/mac-x64/ --human-readable --summarize; fi
fi