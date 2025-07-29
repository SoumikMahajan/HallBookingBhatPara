using HallBookingBhatPara.Domain.Entities;

namespace HallBookingBhatPara.Domain.DTO
{
    public class MultipleModel
    {
        #region :: Global
        public List<DropDownListDTO> dropDownListDTOs { get; set; }
        #endregion

        #region :: Admin
        public List<hall_category_master> CategoryList { get; set; }
        #endregion
    }
}
