return new Dictionary<FlowType, List<NodeDefinition>>
        {
            [FlowType.ContactOrder] = new()
            {
                new() { NodeIndex = 0, Position = "Technician", Role = "技术员", Description = "项目部编制工联单" },
                new() { NodeIndex = 1, Position = "ChiefEngineer", Role = "项目总工", Description = "审核" },
                new() { NodeIndex = 2, Position = "ProjectManager", Role = "项目经理", Description = "审批" },
                new() { NodeIndex = 3, Position = "", Role = "甲方/监理", Description = "签章确认" },
                new() { NodeIndex = 4, Position = "DocumentController", Role = "合同资料部", Description = "归档留存" }
            },
            [FlowType.PriceConfirmation] = new()
            {
                new() { NodeIndex = 0, Position = "MaterialManager", Role = "材料员", Description = "编制认质认价单" },
                new() { NodeIndex = 1, Position = "ProductionManager", Role = "生产经理", Description = "项目级审核" },
                new() { NodeIndex = 2, Position = "ProjectManager", Role = "项目经理", Description = "项目级审批" },
                new() { NodeIndex = 3, Position = "", Role = "公司造价部", Description = "公司级审核" },
                new() { NodeIndex = 4, Position = "", Role = "甲方/监理", Description = "报送确认归档" }
            },
            [FlowType.Visa] = new()
            {
                new() { NodeIndex = 0, Position = "ConstructionWorker", Role = "施工员", Description = "编制工联单确认事项+草签单+监理确认" },
                new() { NodeIndex = 1, Position = "ProductionManager", Role = "生产经理/总工", Description = "编制签证单+调增事项报告" },
                new() { NodeIndex = 2, Position = "ProjectManager", Role = "项目经理", Description = "审核" },
                new() { NodeIndex = 3, Position = "", Role = "工程部", Description = "审核" },
                new() { NodeIndex = 4, Position = "", Role = "造价部", Description = "前置审核（铁律T2）" },
                new() { NodeIndex = 5, Position = "", Role = "分管领导", Description = "审批" },
                new() { NodeIndex = 6, Position = "", Role = "总经理", Description = "终审" },
                new() { NodeIndex = 7, Position = "DocumentController", Role = "归档", Description = "签章齐全后归档→纳入月结算" }
            },
            [FlowType.Claim] = new()
            {
                new() { NodeIndex = 0, Position = "ProductionManager", Role = "生产经理", Description = "编制索赔意向书（28天内）" },
                new() { NodeIndex = 1, Position = "ProjectManager", Role = "项目经理", Description = "审核意向书" },
                new() { NodeIndex = 2, Position = "", Role = "监理/甲方", Description = "上报意向书+证据资料" },
                new() { NodeIndex = 3, Position = "ChiefEngineer", Role = "项目总工", Description = "编制索赔报审单+调增报告（结束后28天内）" },
                new() { NodeIndex = 4, Position = "ProjectManager", Role = "项目经理", Description = "审核报审单" },
                new() { NodeIndex = 5, Position = "", Role = "工程部", Description = "审核" },
                new() { NodeIndex = 6, Position = "", Role = "造价部", Description = "前置审核" },
                new() { NodeIndex = 7, Position = "", Role = "分管领导", Description = "审批" },
                new() { NodeIndex = 8, Position = "", Role = "总经理", Description = "终审" },
                new() { NodeIndex = 9, Position = "DocumentController", Role = "归档", Description = "签章齐全→纳入月结算" }
            },
            [FlowType.DesignChange] = new()
            {
                new() { NodeIndex = 0, Position = "ProjectManager", Role = "项目部", Description = "收到变更通知单（图纸）并审核" },
                new() { NodeIndex = 1, Position = "MaterialManager", Role = "采供部", Description = "审核材料变动影响" },
                new() { NodeIndex = 2, Position = "", Role = "造价部", Description = "审核并编制调增事项报告" },
                new() { NodeIndex = 3, Position = "ProjectManager", Role = "项目部", Description = "返回项目部报监理/甲方签章" },
                new() { NodeIndex = 4, Position = "DocumentController", Role = "归档", Description = "造价部/合同部/采供部分别留存→纳入月结算" }
            },
            // ====== 阶段三：结算管理中心 ======
            [FlowType.MonthlyValuation] = new()
            {
                new() { NodeIndex = 0, Position = "ConstructionWorker", Role = "施工员", Description = "填报本月完成工程量" },
                new() { NodeIndex = 1, Position = "MaterialManager", Role = "材料员", Description = "审核材料用量" },
                new() { NodeIndex = 2, Position = "QualityInspector", Role = "质量员", Description = "审核工程质量" },
                new() { NodeIndex = 3, Position = "SafetyOfficer", Role = "安全员", Description = "审核安全情况" },
                new() { NodeIndex = 4, Position = "ProductionManager", Role = "生产经理", Description = "审核完成量" },
                new() { NodeIndex = 5, Position = "ProjectManager", Role = "项目经理", Description = "审批" },
                new() { NodeIndex = 6, Position = "", Role = "合同部", Description = "审核合同合规" },
                new() { NodeIndex = 7, Position = "", Role = "造价部", Description = "终审核定" },
                new() { NodeIndex = 8, Position = "DocumentController", Role = "归档", Description = "打印签章扫描归档→财务部" }
            },
            [FlowType.MaterialSettlement] = new()
            {
                new() { NodeIndex = 0, Position = "MaterialManager", Role = "材料员", Description = "编制材料月结算单" },
                new() { NodeIndex = 1, Position = "ProductionManager", Role = "生产经理", Description = "审核" },
                new() { NodeIndex = 2, Position = "ProjectManager", Role = "项目经理", Description = "审批" },
                new() { NodeIndex = 3, Position = "", Role = "采供部", Description = "审核" },
                new() { NodeIndex = 4, Position = "", Role = "财务部", Description = "审核" },
                new() { NodeIndex = 5, Position = "", Role = "造价部", Description = "终审核定" },
                new() { NodeIndex = 6, Position = "", Role = "分包单位", Description = "签章确认" },
                new() { NodeIndex = 7, Position = "DocumentController", Role = "归档", Description = "归档录入结算数据库" }
            },
            [FlowType.ConsumptionVerification] = new()
            {
                new() { NodeIndex = 0, Position = "MaterialManager", Role = "材料员", Description = "编制材料消耗台账" },
                new() { NodeIndex = 1, Position = "ProductionManager", Role = "生产经理", Description = "审核" },
                new() { NodeIndex = 2, Position = "ProjectManager", Role = "项目经理", Description = "审批" },
                new() { NodeIndex = 3, Position = "", Role = "采供部", Description = "审核" },
                new() { NodeIndex = 4, Position = "", Role = "财务部", Description = "审核" },
                new() { NodeIndex = 5, Position = "", Role = "造价部", Description = "终审核定" },
                new() { NodeIndex = 6, Position = "DocumentController", Role = "归档", Description = "关联建造合同成本核算" }
            }
        };