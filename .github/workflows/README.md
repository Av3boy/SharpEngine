# SharpEngine Pipelines

## Pull Request pipelines



## Deployment Pipelines

SharpEngine consists of 5 different deployment pipelines:
- Publish Core
- Publish Asset Store
- Publish Web
- Publish Docs
- Publish Launcher

All of these pipelines are responsible of building and exposing the respective components of the system.

### Publish Core
The `publish-core.yml` creates a zip file of the engine and adds a tag to github and the zip to the releases section. That tag is also added for the commit that it was built off. If a tag for the commit this is being run-off is already found, we don't want to create another copy of the essentially the same version so we skip it.

### Publish Asset Store
The `publish-asset-store.yml` is responsible of deploying the Asset Store web interface, API and Database into Azure.

All Asset Store's resources can be found under the `rg-asset-store` resource group.
TODO: Enable from selected access ips 

### Publish Web
The `publish-web.yml` is responsible of (similarly to the Asset Store) deploying the SharpEngine web interface, API and Database into Azure.

All SharpEngine Web's resources can be found under the `rg-sharpengine-web` resource group.

### Publish Docs
The `publish-docs.yml` deploys the documentation files under the ´./docs´ in the root of the repository into the `docs.sharpengine.com` site.

### Publish Launcher
The ``