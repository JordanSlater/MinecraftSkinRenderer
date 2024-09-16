#!/bin/bash

tag=$(git describe --exact-match --tags)

echo Generating release for tag: $tag

cd build

zip -r ../MinecraftSkinRenderer_$tag.zip .


