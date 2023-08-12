#!/bin/bash

project_path=$(pwd)

genesis_block_path="file://$project_path/data/genesis.bin"
store_uri="rocksdb+file://$project_path/data/store?secure=false"

local_json_file="$project_path/Savor22b/appsettings.local.json"
json_file="$project_path/Savor22b/appsettings.json"

if [ ! -f $local_json_file ]; then
    cp $json_file $local_json_file
fi

jq ".GenesisBlockPath = \"$genesis_block_path\" | .StoreUri = \"$store_uri\"" $local_json_file >temp.json && mv temp.json $local_json_file
