import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44302/',
  redirectUri: baseUrl,
  clientId: 'ExampleAbpApplicationLayer_App',
  responseType: 'code',
  scope: 'offline_access ExampleAbpApplicationLayer',
  requireHttps: true,
  impersonation: {
    tenantImpersonation: true,
    userImpersonation: true,
  }
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'ExampleAbpApplicationLayer',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44302',
      rootNamespace: 'ExampleAbpApplicationLayer',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
