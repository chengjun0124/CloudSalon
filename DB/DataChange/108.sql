IF EXISTS (select 1 from ConsumedServices group by AppointmentId having COUNT(AppointmentId) > 1)
BEGIN
	RAISERROR (66666, -- Message id.
           16, -- Severity,
           1 -- State,
           ) ;  
           
    print 'ConsumedServices中的AppointmentId应该没有重复值，需查清楚原因，消除重复值后，继续执行'
    
    --以下是查询重复值的脚本
 --   select cs.*
	--from ConsumedServices cs
	--where ConsumedServiceStatusId=2 and cs.appointmentid is not null
END
ELSE
BEGIN	
	IF EXISTS (select 1
	from ConsumedServices cs
	left join AppointmentFlows af
	on af.AppointmentId=cs.AppointmentId
	and af.AppointmentStatusId=4 
	where ConsumedServiceStatusId=1 and cs.appointmentid is not null
	and af.AppointmentFlowId is null)
	BEGIN
		RAISERROR (66667, -- Message id.
			   16, -- Severity,
			   1 -- State,
			   ) ;  
	           
		print '“等待用户确认”的消费单，未找到对应的预约状态为“已完成的预约”'
		return
	END
	
	IF EXISTS (select 1
	from ConsumedServices cs
	left join AppointmentFlows af
	on af.AppointmentId=cs.AppointmentId
	and af.AppointmentStatusId=4 
	where ConsumedServiceStatusId=2 and cs.appointmentid is not null
	and af.AppointmentFlowId is null)
	BEGIN
		RAISERROR (66668, -- Message id.
			   16, -- Severity,
			   1 -- State,
			   ) ;  
	           
		print '“用户已确认”的消费单，未找到对应的预约状态为“已完成的预约”'
		return
	END
	
	--删除“等待用户确认”的消费单所关联的预约状态，该状态为“已完成预约”
	delete af
	from ConsumedServices cs
	inner join AppointmentFlows af
	on af.AppointmentId=cs.AppointmentId
	where ConsumedServiceStatusId=1 and cs.appointmentid is not null
	and af.AppointmentStatusId=4
	
	
	--更新“用户已确认”的消费单所关联的AppointmentFlows.AppointmentStatusId为4的数据，把数据的CreatedDate设为ConsumedServices.UserConfirmedDate
	--意思是只有用户确认消费单后，预约状态才更新成“预约已完成”
	update af
	set af.CreatedDate=cs.UserConfirmedDate
	from ConsumedServices cs
	inner join AppointmentFlows af
	on af.AppointmentId=cs.AppointmentId
	where ConsumedServiceStatusId=2 and cs.appointmentid is not null
	and af.AppointmentStatusId=4
	
END