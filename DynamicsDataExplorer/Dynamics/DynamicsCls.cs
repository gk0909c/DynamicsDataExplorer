using System;
using System.Collections.Generic;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;

namespace DynamicsDataExplorer.Dynamics
{
    /// <summary>
    /// Dynamics接続用のクラスです。
    /// </summary>
    class DynamicsCls
    {
        private OrganizationService _service;

        /// <summary>
        /// コンストラクタ。dynamicsとの接続を確立する。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="url"></param>
        public DynamicsCls(string user, string pass, string url)
        {
            string connetString = String.Format("Url={0}; Username={1}; Password={2};",
                url,
                user,
                pass);

            CrmConnection connection = CrmConnection.Parse(connetString);
            _service = new OrganizationService(connection);

            WhoAmIResponse res = (WhoAmIResponse)_service.Execute(new WhoAmIRequest());
        }

        /// <summary>
        /// 全てのエンティティの名前と表示名を取得します。
        /// </summary>
        /// <returns></returns>
        public EntityMetadata[] getAllEntity()
        {
            RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };
            RetrieveAllEntitiesResponse entityResponse = (RetrieveAllEntitiesResponse)_service.Execute(request);

            return entityResponse.EntityMetadata;
        }

        /// <summary>
        /// エンティティのフィールドを取得します。
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public AttributeMetadata[] getAttributes(string entityName)
        {
            MetadataFilterExpression EntityFilter = new MetadataFilterExpression();
            EntityFilter.Conditions.Add(new MetadataConditionExpression(
                "LogicalName",
                MetadataConditionOperator.Equals,
                entityName));

            MetadataPropertiesExpression EntityProperties = new MetadataPropertiesExpression()
            {
                AllProperties = true
            };

            MetadataPropertiesExpression AttributeProperties = new MetadataPropertiesExpression()
            {
                AllProperties = true
            };

            LabelQueryExpression labelQuery = new LabelQueryExpression();
            labelQuery.FilterLanguages.Add(1041);

            EntityQueryExpression entityQueryExpression = new EntityQueryExpression()
            {
                Criteria = EntityFilter, // エンティティのフィルター
                Properties = EntityProperties, // エンティティのプロパティ指定
                AttributeQuery = new AttributeQueryExpression()
                {
                    Properties = AttributeProperties // フィールドのプロパティの指定
                },
                LabelQuery = labelQuery // 表示言語の指定
            };

            RetrieveMetadataChangesResponse response = (RetrieveMetadataChangesResponse)_service.Execute(
                new RetrieveMetadataChangesRequest()
                {
                    Query = entityQueryExpression
                }
            );

            return response.EntityMetadata[0].Attributes;
        }

        /// <summary>
        /// データ全件を取得します。
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public EntityCollection getData(string entityName)
        {
            return getData(entityName, null);
        }

        /// <summary>
        /// データを取得します。
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public EntityCollection getData(string entityName, ConditionExpression cond)
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = entityName;
            query.ColumnSet = new ColumnSet(true);

            if (cond != null)
            {
                query.Criteria.AddCondition(cond);
            }

            return _service.RetrieveMultiple(query);
        }
    }
}
