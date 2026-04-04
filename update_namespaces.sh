#!/bin/bash

# Update namespaces in Workshop.Application
find src/Workshop.Application/Features -name "*.cs" | while read -r filepath; do
    dirpath=$(dirname "$filepath")
    relative_path=${dirpath#src/}
    
    # Convert path to namespace format (e.g., Workshop.Application/Features/ServiceRequests/Commands -> Workshop.Application.Features.ServiceRequests.Commands)
    new_namespace=$(echo "$relative_path" | tr '/' '.')
    
    # Replace the existing namespace declaration
    # Supports both block-scoped and file-scoped
    sed -i -E "s/namespace Workshop\.Application\.[a-zA-Z0-9_.]+/namespace $new_namespace/g" "$filepath"
done

# We also need to fix using statements everywhere because types moved
# Actually, since there are so many across API, let's let dotnet build tell us, or just sed replace commonly used ones.
